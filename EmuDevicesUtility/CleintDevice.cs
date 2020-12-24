using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using DiplomApp.Server.SetOfConstants;


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
        public event EventHandler AfterSendMessage;
        private const string CONNECTION = DiplomApp.Server.SetOfConstants.Topics.CONNECTION;
        private readonly MqttClient client;
        private readonly Controller controller;
        private readonly CancellationTokenSource cancellation;


        public IReadOnlyList<string> Topics { get; }
        public bool IsConnected { get; private set; }

        public ClientDevice(Controller controller)
        {
            this.controller = controller;
            Topics = new List<string>() {
               "#"
            }.AsReadOnly();

            client = new MqttClient("localhost");
            foreach (var topic in Topics)
            {
                client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            client.Connect(this.controller.ID);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            IsConnected = false;
            cancellation = new CancellationTokenSource();

            this.controller.PropertyChanged += Controller_PropertyChanged;
        }

        private void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var property = sender.GetType().GetProperty(e.PropertyName);
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
            client.Publish(CONNECTION, Encoding.UTF8.GetBytes(res));
        }
        public void SendDisconnect()
        {
            if (!client.IsConnected) return;
            var message = new
            {
                Message_Type = MessageTypes.REQUSET_TO_DISCONNECT,
                controller.ID
            };
            var res = JsonConvert.SerializeObject(message);
            client.Publish(CONNECTION, Encoding.UTF8.GetBytes(res));
            client.Disconnect();
            if (!IsConnected) return;
            IsConnected = false;
            cancellation.Cancel();
        }
        private async void StartDistributionOfValues()
        {
            void Action(CancellationToken x)
            {
                while (!x.IsCancellationRequested)
                {
                    DistributeValue();
                    AfterSendMessage?.Invoke(this, new EventArgs());
                    Thread.Sleep((int)Properties.Settings.Default.SendMessageDelay.TotalMilliseconds);
                }
            }
            await Task.Run(() => Action(cancellation.Token), cancellation.Token);
        }
        private void StopDistributionOfValues()
        {
            cancellation.Cancel();
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic != CONNECTION 
                || !(JsonConvert.DeserializeObject(
                    Encoding.UTF8.GetString(e.Message), 
                    typeof(Dictionary<string, string>))
                    is Dictionary<string, string> message)) 
                return;

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

                            controller.Options.SetOptions(sensorId, valueTo);
                        }
                    }
                }
            }
        }

        private string GetCurrentTopic()
        {
            var res = "Devices/";
            switch (controller)
            {
                case Sensor _:
                    res += "Sensors";
                    break;
                case Switch _:
                    res += "Switches";
                    break;
            }
            return res;
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
            client.Publish(GetCurrentTopic(), Encoding.UTF8.GetBytes(res));
        }

        ~ClientDevice()
        {
            StopDistributionOfValues();
            SendDisconnect();
        }
    }
}
