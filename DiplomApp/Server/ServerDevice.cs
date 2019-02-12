using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using MQTTnet.Server;
using MQTTnet;

namespace DiplomApp.Server
{
    class ServerDevice
    {
        private static ServerDevice instance;
        private readonly IMqttServerOptions serverOptions;
        private readonly IMqttServer server;
        private readonly MqttClient client;
        private readonly CancellationTokenSource cancellationRun;
        private readonly IEnumerable<Type> SupportedRequestHandlers;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static ServerDevice Instance
        {
            get
            {
                if (instance == null) instance = new ServerDevice();
                return instance;
            }
        }
        public Guid ID { get; }
        public IReadOnlyList<string> Topics { get; }

        private ServerDevice()
        {
            //Инициализация полей и свойств
            ID = Guid.NewGuid();
            Topics = new List<string>() {
                SetOfConstants.Topics.CONNECTION
            }.AsReadOnly();

            //Инициализация сервера
            server = new MqttFactory().CreateMqttServer();
            serverOptions = new MqttServerOptions();

            //Инициализация MQTT клиента
            client = new MqttClient("localhost");
            foreach (var topic in Topics)
            {
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            client.MqttMsgPublishReceived += MqttMsgPublishReceived;

            //Инициализация обработчиков запросов
            var interfaceName = typeof(IRequestHandler).Name;            
            var types = Assembly.GetExecutingAssembly().GetTypes().Where
                (x => x.GetInterface(interfaceName) != null && x.GetCustomAttributes(typeof(RequestTypeAttribute), true).Length == 1);
            SupportedRequestHandlers = types.Where
                (x => x.GetCustomAttributes(typeof(RequestTypeAttribute), true).Length == 1);

            //Создание токена отмены асинхронной задачи
            cancellationRun = new CancellationTokenSource();
        }

        public async void RunAsync()
        {
            logger.Info("Запуск сервера");
            await server.StartAsync(serverOptions);            
            logger.Debug("Попытка подключения к mqtt серверу");
            client.Connect(ID.ToString());
            var token = cancellationRun.Token;
            token.Register(() =>
            {
                logger.Debug("Закрытие асинхронного потока для сервера");
            });
            await Task.Run(() =>
            {
                logger.Debug("Запущен асинхронный поток для сервера");
                while (true)
                {
                    SendBroadcast();
                    Thread.Sleep(10000);
                }
            }, cancellationRun.Token);
        }
        public async void StopAsync()
        {
            if (!client.IsConnected) return;
            logger.Info("Останока работы сервера");
            cancellationRun.Cancel();
            client.Disconnect();
            await server.StopAsync();
        }
        public void SendMessage(string message, string topic)
        {
            client.Publish(topic, Encoding.UTF8.GetBytes(message));
        }
        public void SendMessage(Dictionary<string, string> keyValuePairs, string topic)
        {
            var str = JsonConvert.SerializeObject(keyValuePairs);
            client.Publish(topic, Encoding.UTF8.GetBytes(str));
        }

        private void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Dictionary<string, string> message = null;
            try
            {
                var jsonMessage = Encoding.UTF8.GetString(e.Message);
                message = JsonConvert.DeserializeObject(jsonMessage, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
            }
            catch (Exception w)
            {
                logger.Error(w, "Не удалось распарсить данные, возможно нарушение структуры данных");
                return;
            }

            message.TryGetValue("Message_Type", out string req);
            logger.Debug($"Получено сообщение из топика { e.Topic}. Тип сообщения: {req}");
            if (req == SetOfConstants.MessageTypes.BROADCAST_CONNECTION_REQUSET ||
                req == SetOfConstants.MessageTypes.PERMIT_TO_CONNECT
                ) return;

            message.Add("Topic", e.Topic);
            HandleRequest(message);

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
        private void HandleRequest(Dictionary<string, string> keyValuePairs)
        {            
            keyValuePairs.TryGetValue("Message_Type", out string msgType);
            var type = SupportedRequestHandlers.FirstOrDefault(x => (x.GetCustomAttribute(typeof(RequestTypeAttribute)) as RequestTypeAttribute).MessageType == msgType);
            if (type == null)
                throw new NullReferenceException($"Не удалось найти обработчик события соответствующий запросу: {msgType}");
            var prop = type.GetProperty("Instance");
            if (prop == null)
                throw new NotImplementedException($"В классе {type.Name} не реализован паттерн Singleton");
            var getClass = prop.GetMethod.Invoke(null, null) as IRequestHandler;
            keyValuePairs.Remove("Message_Type");
            getClass.Execute(keyValuePairs);
        }
    }
}
