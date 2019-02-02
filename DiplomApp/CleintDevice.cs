using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace MQTTTestApp
{
    class ClientDevice
    {
        const string CONNECTION = SetOfConstants.Topics.CONNECTION;
        readonly MqttClient client;
        readonly Controller controller;

        public IReadOnlyList<string> Topics { get; }
        public bool IsConnected { get; private set; }

        public ClientDevice(Controller _controller)
        {
            controller = _controller;
            Topics = new List<string>() {
                CONNECTION,
                GetTopic(controller.Type)
            }.AsReadOnly();

            client = new MqttClient("localhost");
            foreach (var topic in Topics)
            {
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }            
            client.Connect(controller.ID);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            IsConnected = false;
        }

        public void SendConnect()
        {
            var message = new
            {
                Message_Type = SetOfConstants.MessageTypes.REQUSET_TO_CONNECT,
                controller.ID,
                controller.Name,
                controller.Type,
                controller.Value
            };
            var res = JsonConvert.SerializeObject(message);
            client.Publish(CONNECTION, Encoding.UTF8.GetBytes(res));
        }
        public void SendDisconnect()
        {
            var message = new
            {
                Message_Type = SetOfConstants.MessageTypes.REQUSET_TO_DISCONNECT,
                controller.ID
            };
            var res = JsonConvert.SerializeObject(message);
            client.Publish(CONNECTION, Encoding.UTF8.GetBytes(res));
            IsConnected = false;
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == CONNECTION)
            {
                var message = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Message))
                    as Dictionary<string, string>;

                message.TryGetValue("Message_Type", out string val);

                if (val == SetOfConstants.MessageTypes.BROADCAST_CONNECTION_REQUSET) SendConnect();
                if (val == SetOfConstants.MessageTypes.PERMIT_TO_CONNECT) IsConnected = true;
            }
        }
        string GetTopic(CType type)
        {
            switch (type)
            {
                case CType.Switch: return SetOfConstants.Topics.SWITCHES;
                case CType.Termometer: return SetOfConstants.Topics.TERMOMETERS;
                default: throw new InvalidEnumArgumentException("Не удалость преобразовать перечисление в топик");
            }
        }

        ~ClientDevice()
        {
            client.Disconnect();
            SendDisconnect();
        }
    }
}
