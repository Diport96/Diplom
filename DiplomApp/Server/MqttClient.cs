using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using NLog;

namespace DiplomApp.Server
{
    class MqttClient : IMqttClientComponent
    {
        private readonly Guid Id;
        private readonly IMqttClient client;
        private readonly MqttClientOptions clientOptions;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly object _asyncLocker;

        public bool IsRun { get; private set; }

        public MqttClient(Func<MqttApplicationMessageReceivedEventArgs, Task> callback)
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
            client.ApplicationMessageReceivedAsync += callback;
        }

        // Constructor for testing — bypasses factory creation
        internal MqttClient(
            IMqttClient client,
            MqttClientOptions clientOptions,
            Func<MqttApplicationMessageReceivedEventArgs, Task> callback)
        {
            Id = Guid.NewGuid();
            IsRun = false;
            _asyncLocker = new object();
            this.client = client;
            this.clientOptions = clientOptions;
            client.ApplicationMessageReceivedAsync += callback;
        }

        public async Task<bool> RunAsync()
        {
            lock (_asyncLocker)
            {
                if (IsRun) return true;
                IsRun = true;
            }

            async Task<bool> TryConnect()
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

            if (!await TryConnect()) return false;

            await client.SubscribeAsync(
                new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic("#").WithExactlyOnceQoS())
                    .Build());

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
            await client.DisconnectAsync().ConfigureAwait(false);
        }

        public async Task SendMessage(string jsonMessage, string topic)
        {
            await client.PublishStringAsync(topic, jsonMessage);
        }

        public async Task SendMessage(Dictionary<string, string> keyValuePairs, string topic)
        {
            var str = JsonConvert.SerializeObject(keyValuePairs);
            await client.PublishStringAsync(topic, str);
        }
    }
}
