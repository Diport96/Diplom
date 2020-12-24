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
        Termometer
    }

    class ClientDevice
    {
        public event EventHandler MessageSended;
        const string CONNECTION = DiplomApp.Server.SetOfConstants.Topics.CONNECTION;
        readonly MqttClient client;
        readonly Controller controller;
        readonly CancellationTokenSource cancellation;


        public IReadOnlyList<string> Topics { get; }
        public bool IsConnected { get; private set; }

        public ClientDevice(Controller _controller)
        {
            controller = _controller;
            Topics = new List<string>() {
               "#"
            }.AsReadOnly();

            client = new MqttClient("localhost");
            foreach (var topic in Topics)
            {
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            client.Connect(controller.ID);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            IsConnected = false;
            cancellation = new CancellationTokenSource();

            controller.PropertyChanged += Controller_PropertyChanged;
        }

        private void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var property = sender.GetType().GetProperty(e.PropertyName);
            var val = property.GetValue(sender);

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
            Action<CancellationToken> action = (x) =>
            {
                while (!x.IsCancellationRequested)
                {
                    DistributeValue();
                    MessageSended?.Invoke(this, new EventArgs());
                    Thread.Sleep((int)Properties.Settings.Default.SendMessageDelay.TotalMilliseconds);
                }
            };
            await Task.Run(() => action(cancellation.Token), cancellation.Token);
        }
        private void StopDistributionOfValues()
        {
            cancellation.Cancel();
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Message), typeof(Dictionary<string, string>))
                as Dictionary<string, string>;

            if (e.Topic == CONNECTION)
            {
                message.TryGetValue("Message_Type", out string val);

                if (val == MessageTypes.PERMIT_TO_CONNECT)
                {
                    message.TryGetValue("ID", out string id);
                    if (id == controller.ID)
                    {
                        IsConnected = true;

                        if (controller is Sensor || controller is Termometer)
                        {
                            StartDistributionOfValues();
                        }
                        else if (controller is Switch)
                        {
                            message.TryGetValue("Control", out string controlType);

                            if (controlType == SwitchControl.No.ToString())
                            {
                                controller.Options.SetOptions();
                            }
                            else if (controlType == SwitchControl.SwitchToDelay.ToString())
                            {
                                message.TryGetValue("Delay", out string _delay);
                                message.TryGetValue("ValueTo", out string _valueTo);
                                int.TryParse(_delay, out int delay);
                                bool.TryParse(_valueTo, out bool valueTo);

                                controller.Options.SetOptions(delay, valueTo);
                            }
                            else if (controlType == SwitchControl.SwitchToSignal.ToString())
                            {
                                message.TryGetValue("SensorId", out string sensorId);
                                message.TryGetValue("ValueTo", out string _valueTo);
                                bool.TryParse(_valueTo, out bool valueTo);

                                controller.Options.SetOptions(sensorId, valueTo);
                            }
                        }
                    }
                }
            }
        }

        private string GetCurrentTopic()
        {
            var res = "Devices/";
            if (controller is Sensor)
            {
                res += "Sensros";
            }
            else if (controller is Switch)
            {
                res += "Switches";
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
