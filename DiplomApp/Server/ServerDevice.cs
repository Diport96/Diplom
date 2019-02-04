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
        public delegate void ControllerConnectionHandler(object sender, Controller e);
        private readonly MqttClient client;
        public readonly Guid ID;
        private static ServerDevice instance;
        public IReadOnlyList<string> Topics { get; }
        public event ControllerConnectionHandler OnControllerConnected;

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
        }

        public static ServerDevice GetInstance()
        {
            if (instance == null) instance = new ServerDevice();
            return instance;
        }
        public void Run()
        {
            SendBroadcast();
        }
        void SendBroadcast()
        {
            var message = new
            {
                Message_Type = SetOfConstants.MessageTypes.BROADCAST_CONNECTION_REQUSET
            };
            var res = JsonConvert.SerializeObject(message, Formatting.Indented);
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
                var message = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Message), typeof(Dictionary<string, string>))
                    as Dictionary<string, string>;
                message.TryGetValue("Message_Type", out string req);

                if (req == SetOfConstants.MessageTypes.REQUSET_TO_CONNECT)
                {
                    message.TryGetValue("ID", out string id);
                    message.TryGetValue("Name", out string name);
                    message.TryGetValue("Type", out string type);
                    message.TryGetValue("Value", out string value);                    
                    SendConnack(id);
                    OnControllerConnected?.Invoke(this, BuildController(id,name,type,value));
                }
            }
        }
        private Controller BuildController(string id, string name, string type, string value)
        {
            var cType = (CType)Enum.Parse(typeof(CType), type);
            if(cType == CType.Termometer)
            {
                var resID = Guid.Parse(id);
                var resValue = double.Parse(value);
                return new Termometer(resID, name, resValue);
            }
            else
            {
                throw new InvalidEnumArgumentException(); //MessageBox Alert
            }
        }

        ~ServerDevice()
        {
            client.Disconnect();
        }
    }
}
