using DiplomApp.Server.RequsestHandlers;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    class MqttClient : IMqttComponent
    {
        private Guid Id;
        private readonly IMqttClient client;
        private readonly IMqttClientOptions clientOptions;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly TopicFilter topic;
        private readonly object _asyncLocker;

        public bool IsRun { get; private set; }

        public MqttClient()
        {
            Id = Guid.NewGuid();
            IsRun = false;
            _asyncLocker = new object();
            var mqttFactory = new MqttFactory();
            client = mqttFactory.CreateMqttClient();
            clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(Id.ToString())
                .WithTcpServer(Properties.Settings.Default.ServerDomain)
                .Build();
            topic = new TopicFilterBuilder()
                  .WithTopic("#")
                  .WithExactlyOnceQoS()
                  .Build();
            client.ApplicationMessageReceived += MqttMsgPublishReceived;
        }

        /// <summary>
        /// Конструктор для тестирования
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientOptions"></param>
        /// <param name="logger"></param>
        internal MqttClient(IMqttClient client, IMqttClientOptions clientOptions) : this()
        {
            this.client = client;
            this.clientOptions = clientOptions;
        }

        public async Task<bool> RunAsync()
        {
            lock (_asyncLocker)
            {
                if (IsRun) return true;
                IsRun = true;
            }
            async Task<bool> tryConnect()
            {
                logger.Debug("Попытка подключения к mqtt серверу");
                try
                {
                    await client.ConnectAsync(clientOptions);
                }
                catch (Exception e)
                {
                    logger.Warn(e, "Не удалось осуществить соединение с сервером");
                    return false;
                }
                return true;
            }

            if (!await tryConnect()) return false;
            await client.SubscribeAsync(topic);
            IsRun = true;
            return true;
        }
        public async Task StopAsync()
        {
            lock (_asyncLocker)
            {
                if (!IsRun) return;
                IsRun = false;
            }
            await client.DisconnectAsync()
                .ConfigureAwait(false);
        }

        public async Task SendMessage(string jsonMessage, string topic)
        {
            await client.PublishAsync(topic, jsonMessage)
                .ConfigureAwait(false);
        }
        public async Task SendMessage(Dictionary<string, string> keyValuePairs, string topic)
        {
            var str = JsonConvert.SerializeObject(keyValuePairs);
            await client.PublishAsync(topic, str)
                .ConfigureAwait(false);
        }

        private void MqttMsgPublishReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Dictionary<string, string> message;
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
            if (req == SetOfConstants.MessageTypes.PERMIT_TO_CONNECT) return;

            message.Add("Topic", e.ApplicationMessage.Topic);
            try
            {
                var handler = BaseRequestHandler.GetRequestHandler(message);
                message.Remove("Message_Type");
                handler.Run(message);
            }
            catch (HandlerNotFindException w)
            {
                logger.Error(w.Message);
            }
        }
    }
}
