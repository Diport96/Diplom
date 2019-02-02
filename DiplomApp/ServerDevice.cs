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
        private static ServerDevice instance;
        private readonly MqttClient client;
        public readonly Guid ID;
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
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            SendBroadcast();

        }

        public static ServerDevice GetInstance()
        {
            if (instance == null) instance = new ServerDevice();
            return instance;
        }
        void SendBroadcast()
        {
            var message = new
            {
                Message_Type = SetOfConstants.MessageTypes.BROADCAST_CONNECTION_REQUSET
            };
            var res = JsonConvert.SerializeObject(message);
            client.Publish(Topics[0], Encoding.UTF8.GetBytes(res));
        }
        void SendConnack(string id)
        {
            var message = new
            {
                Message_Type = SetOfConstants.MessageTypes.PERMIT_TO_CONNECT,
                ID = id
            };
            var res = JsonConvert.SerializeObject(message);
            client.Publish(Topics[0], Encoding.UTF8.GetBytes(res));
        }
        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == Topics[0])
            {
                var message = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Message))
                    as Dictionary<string, string>;
                message.TryGetValue("Message_Type", out string val);

                if (val == SetOfConstants.MessageTypes.REQUSET_TO_CONNECT)
                {
                    message.TryGetValue("ID", out string id);
                    SendConnack(id);
                }
            }
        }

        ~ServerDevice()
        {
            client.Disconnect();
        }
    }
}
