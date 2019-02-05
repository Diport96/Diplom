using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace DiplomApp
{
    class ServerDevice
    {
        public delegate void ControllerConnectionHandler(object sender, Dictionary<string, string> args);
        public event ControllerConnectionHandler OnControllerConnected;
        private readonly MqttClient client;
        private static ServerDevice instance;
        public Guid ID { get; }
        public IReadOnlyList<string> Topics { get; }

        private ServerDevice()
        {
            ID = Guid.NewGuid();
            Topics = new List<string>() {
                SetOfConstants.Topics.CONNECTION
            }.AsReadOnly();

            client = new MqttClient("localhost");
            foreach (var topic in Topics)
            {
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            client.Connect(ID.ToString());
            client.MqttMsgPublishReceived += MqttMsgPublishReceived;
        }

        public static ServerDevice GetInstance()
        {
            if (instance == null) instance = new ServerDevice();
            return instance;
        }
        public void Run()
        {
            SendBroadcast();
        } // Сделать асинхронным методом
        private void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == Topics[0])
            {
                var message = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Message), typeof(Dictionary<string, string>))
                    as Dictionary<string, string>;
                message.TryGetValue("Message_Type", out string req);

                if (req == SetOfConstants.MessageTypes.REQUSET_TO_CONNECT)
                {
                    message.TryGetValue("ID", out string id);
                    message.Remove("Message_Type");

                    OnControllerConnected?.Invoke(this, message);
                    SendConnack(id);
                }
            }
        }
        private void SendBroadcast()
        {
            var message = new
            {
                Message_Type = SetOfConstants.MessageTypes.BROADCAST_CONNECTION_REQUSET
            };
            var res = JsonConvert.SerializeObject(message, Formatting.Indented);
            client.Publish(Topics[0], Encoding.UTF8.GetBytes(res));
        }
        private void SendConnack(string id)
        {
            var message = new
            {
                Message_Type = SetOfConstants.MessageTypes.PERMIT_TO_CONNECT,
                ID = id
            };
            var res = JsonConvert.SerializeObject(message);
            client.Publish(Topics[0], Encoding.UTF8.GetBytes(res));
        }
        
        ~ServerDevice()
        {
            client.Disconnect();
        }
    }
}
