using DiplomApp.Server.SetOfConstants;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;

namespace EmuDevicesUtility
{
    [Flags]
    public enum CType
    {
        Switch,
        Sensor,
        Thermometer
    }

    internal class ClientDevice
    {
        public event EventHandler? AfterSendMessage;
        private const string CONNECTION = Topics.CONNECTION;
        private readonly IMqttClient _mqttClient;
        private readonly Controller controller;
        private readonly CancellationTokenSource cancellation;

        public IReadOnlyList<string> Topics { get; } = new List<string> { "#" }.AsReadOnly();
        public bool IsConnected { get; private set; }

        public ClientDevice(Controller controller)
        {
            this.controller = controller;
            cancellation = new CancellationTokenSource();

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;

            var options = new MqttClientOptionsBuilder()
                .WithClientId(this.controller.ID)
                .WithTcpServer("localhost")
                .Build();

            _mqttClient.ConnectAsync(options).GetAwaiter().GetResult();

            _mqttClient.SubscribeAsync(
                new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic("#").WithExactlyOnceQoS())
                    .Build()
            ).GetAwaiter().GetResult();

            IsConnected = false;
            this.controller.PropertyChanged += Controller_PropertyChanged;
        }

        private void Controller_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var property = sender?.GetType().GetProperty(e.PropertyName ?? string.Empty);
            property?.GetValue(sender);
        }

        public void SendConnect()
        {
            var cData = JsonConvert.SerializeObject(new
            {
                controller.ID,
                controller.Name,
                controller.Value
            });

            var message = new
            {
                Message_Type = MessageTypes.REQUSET_TO_CONNECT,
                controller.Type,
                Class = cData
            };
            var res = JsonConvert.SerializeObject(message);
            _mqttClient.PublishStringAsync(CONNECTION, res).GetAwaiter().GetResult();
        }

        public void SendDisconnect()
        {
            if (!_mqttClient.IsConnected) return;
            var message = new
            {
                Message_Type = MessageTypes.REQUSET_TO_DISCONNECT,
                controller.ID
            };
            var res = JsonConvert.SerializeObject(message);
            _mqttClient.PublishStringAsync(CONNECTION, res).GetAwaiter().GetResult();
            _mqttClient.DisconnectAsync().GetAwaiter().GetResult();
            if (!IsConnected) return;
            IsConnected = false;
            cancellation.Cancel();
        }

        private async void StartDistributionOfValues()
        {
            void Action(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    DistributeValue();
                    AfterSendMessage?.Invoke(this, EventArgs.Empty);
                    Thread.Sleep((int)Properties.Settings.Default.SendMessageDelay.TotalMilliseconds);
                }
            }
            await Task.Run(() => Action(cancellation.Token), cancellation.Token);
        }

        private void StopDistributionOfValues()
        {
            cancellation.Cancel();
        }

        private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            if (e.ApplicationMessage.Topic != CONNECTION)
                return Task.CompletedTask;

            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            if (JsonConvert.DeserializeObject(payload, typeof(Dictionary<string, string>))
                    is not Dictionary<string, string> message)
                return Task.CompletedTask;

            message.TryGetValue("Message_Type", out var val);

            if (val == MessageTypes.PERMIT_TO_CONNECT)
            {
                message.TryGetValue("ID", out var id);
                if (id == controller.ID)
                {
                    IsConnected = true;

                    if (controller is Sensor || controller is Termometer)
                    {
                        StartDistributionOfValues();
                    }
                    else if (controller is Switch)
                    {
                        message.TryGetValue("Control", out var controlType);

                        if (controlType == SwitchControl.No.ToString())
                        {
                            controller.Options.SetOptions();
                        }
                        else if (controlType == SwitchControl.SwitchToDelay.ToString())
                        {
                            message.TryGetValue("Delay", out var delayInput);
                            message.TryGetValue("ValueTo", out var valueInput);
                            int.TryParse(delayInput, out var delay);
                            bool.TryParse(valueInput, out var valueTo);
                            controller.Options.SetOptions(delay, valueTo);
                        }
                        else if (controlType == SwitchControl.SwitchToSignal.ToString())
                        {
                            message.TryGetValue("SensorId", out var sensorId);
                            message.TryGetValue("ValueTo", out var valueInput);
                            bool.TryParse(valueInput, out var valueTo);
                            controller.Options.SetOptions(sensorId!, valueTo);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private string GetCurrentTopic()
        {
            return controller switch
            {
                Sensor => "Devices/Sensors",
                Switch => "Devices/Switches",
                _ => "Devices/Other"
            };
        }

        public void DistributeValue()
        {
            var message = new
            {
                Message_Type = MessageTypes.DISTRIBUTION_OF_VALUES,
                controller.ID,
                Date = DateTime.Now,
                controller.Value
            };
            var res = JsonConvert.SerializeObject(message);
            _mqttClient.PublishStringAsync(GetCurrentTopic(), res).GetAwaiter().GetResult();
        }

        ~ClientDevice()
        {
            StopDistributionOfValues();
            SendDisconnect();
        }
    }
}
