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
using MQTTnet.Server;
using MQTTnet;
using MQTTnet.Client;

namespace DiplomApp.Server
{
    class ServerDevice
    {
        private static ServerDevice instance;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly IEnumerable<Type> SupportedRequestHandlers;
        private readonly object _locker = new object();
        private readonly CancellationTokenSource cancellationSource;
        private readonly IMqttServerOptions serverOptions;
        private readonly IMqttClientOptions clientOptions;
        private readonly IMqttServer server;
        private readonly IMqttClient client;

        public static ServerDevice Instance
        {
            get
            {
                if (instance == null) instance = new ServerDevice();
                return instance;
            }
        }
        public Guid ID { get; }
        public bool IsRun { get; private set; }

        private ServerDevice()
        {
            var mqttFactory = new MqttFactory();

            //Инициализация полей и свойств
            ID = Guid.NewGuid();
            IsRun = false;

            //Инициализация сервера
            server = mqttFactory.CreateMqttServer();
            serverOptions = new MqttServerOptions();

            //Инициализация MQTT клиента
            client = mqttFactory.CreateMqttClient();
            clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(ID.ToString())
                .WithTcpServer(Properties.Settings.Default.ServerDomain)
                .Build();
            client.ApplicationMessageReceived += MqttMsgPublishReceived;

            //Создание токена отмены асинхронной задачи для остановки работы сервера
            cancellationSource = new CancellationTokenSource();
        }
        static ServerDevice()
        {
            var interfaceName = typeof(IRequestHandler).Name;
            var types = Assembly.GetExecutingAssembly().GetTypes().Where
                (x => x.GetInterface(interfaceName) != null && x.GetCustomAttributes(typeof(RequestTypeAttribute), true).Length == 1);
            SupportedRequestHandlers = types.Where
               (x => x.GetCustomAttributes(typeof(RequestTypeAttribute), true).Length == 1);
        }

        public async Task RunAsync()
        {
            Func<Task> sendBroadcast = async () =>
            {
                var message = new
                {
                    Message_Type = SetOfConstants.MessageTypes.BROADCAST_CONNECTION_REQUSET
                };
                var res = JsonConvert.SerializeObject(message, Formatting.Indented);
                await client.PublishAsync(SetOfConstants.Topics.CONNECTION, res);
            };
            Action<CancellationToken> broadcastAction = (x) =>
            {
                logger.Debug("Запущен асинхронный поток для сервера");
                while (!x.IsCancellationRequested)
                {
                    Task.Run(() => sendBroadcast());
                    Thread.Sleep(10000); //!!! Seetings
                }
            };
            Func<Task<bool>> tryConnect = async () =>
            {
                logger.Debug("Попытка подключения к mqtt серверу");
                try
                {
                    await client.ConnectAsync(clientOptions);
                }
                catch (Exception e)
                {
                    logger.Warn(e, "Не удалось подключиться к серверу");
                    return false;
                }
                return true;
            };

            logger.Info("Подключение к удаленному серверу...");
            if (!await tryConnect())
            {
                logger.Info("Запуск локального сервера");
                await server.StartAsync(serverOptions);
                await tryConnect();
            }
            var topic = new TopicFilterBuilder()
                  .WithTopic("#")
                  .WithExactlyOnceQoS()
                  .Build();
            await client.SubscribeAsync(topic);
            var token = cancellationSource.Token;
            token.Register(() =>
            {
                logger.Debug("Закрытие асинхронного потока для сервера");
            });
            IsRun = true;
            new Task(() => broadcastAction(cancellationSource.Token), cancellationSource.Token).Start();
        }
        public async Task StopAsync()
        {
            lock (_locker)
            {
                if (!IsRun) return;
                IsRun = false;
            }
            logger.Info("Останока работы сервера");
            cancellationSource.Cancel();
            if (client.IsConnected)
                await client.DisconnectAsync()
                    .ConfigureAwait(false);
            await server.StopAsync()
                .ConfigureAwait(false);
        }
        public async void SendMessage(string jsonMessage, string topic)
        {
            await client.PublishAsync(topic, jsonMessage);
        }
        public async void SendMessage(Dictionary<string, string> keyValuePairs, string topic)
        {
            var str = JsonConvert.SerializeObject(keyValuePairs);
            await client.PublishAsync(topic, str);
        }

        private void MqttMsgPublishReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Dictionary<string, string> message = null; //!!!
            try
            {
                var jsonMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                message = JsonConvert.DeserializeObject(jsonMessage, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
            }
            catch (Exception w)
            {
                logger.Error(w, "Не удалось распарсить данные, возможно нарушение структуры данных");
                return;
            }

            message.TryGetValue("Message_Type", out string req);
            logger.Trace($"Получено сообщение из топика { e.ApplicationMessage.Topic}. Тип сообщения: {req}");
            if (req == SetOfConstants.MessageTypes.BROADCAST_CONNECTION_REQUSET ||
                req == SetOfConstants.MessageTypes.PERMIT_TO_CONNECT
                ) return;

            message.Add("Topic", e.ApplicationMessage.Topic);
            try
            {
                HandleRequest(message);
            }
            catch (HandlerNotFindException w)
            {
                logger.Error(w.Message);
            }

        }
        private void HandleRequest(Dictionary<string, string> keyValuePairs)
        {
            keyValuePairs.TryGetValue("Message_Type", out string msgType);
            var type = SupportedRequestHandlers.FirstOrDefault(x => (x.GetCustomAttribute(typeof(RequestTypeAttribute)) as RequestTypeAttribute).MessageType == msgType);
            if (type == null)
                throw new HandlerNotFindException($"Не удалось найти обработчик события соответствующий запросу: {msgType}");
            var prop = type.GetProperty("Instance");
            if (prop == null)
                throw new NotImplementedException($"В классе {type.Name} не реализован паттерн Singleton");
            var getClass = prop.GetMethod.Invoke(null, null) as IRequestHandler;
            keyValuePairs.Remove("Message_Type");
            getClass.Execute(keyValuePairs);
        }
    }
}
