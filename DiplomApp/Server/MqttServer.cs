using MQTTnet;
using MQTTnet.Server;
using NLog;

namespace DiplomApp.Server
{
    // Thin adapter interface allows mocking in tests without depending on MQTTnet internals
    internal interface IMqttServerAdapter
    {
        Task StartAsync();
        Task StopAsync();
    }

    internal sealed class MqttNetServerAdapter : IMqttServerAdapter
    {
        private readonly global::MQTTnet.Server.MqttServer _server;

        public MqttNetServerAdapter(global::MQTTnet.Server.MqttServer server) => _server = server;

        public Task StartAsync() => _server.StartAsync();
        public Task StopAsync() => _server.StopAsync();
    }

    class MqttServer : IMqttComponent
    {
        private readonly IMqttServerAdapter server;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly object _asyncLocker;

        public bool IsRun { get; private set; }

        public MqttServer()
        {
            IsRun = false;
            _asyncLocker = new object();
            var options = new MqttServerOptionsBuilder().Build();
            var realServer = new MqttFactory().CreateMqttServer(options);
            server = new MqttNetServerAdapter(realServer);
        }

        internal MqttServer(IMqttServerAdapter serverAdapter)
        {
            IsRun = false;
            _asyncLocker = new object();
            server = serverAdapter;
        }

        public async Task<bool> RunAsync()
        {
            lock (_asyncLocker)
            {
                if (IsRun) return true;
                IsRun = true;
            }
            try
            {
                await server.StartAsync();
            }
            catch (InvalidOperationException e)
            {
                logger.Error(e, e.Message);
                return false;
            }
            logger.Info("Сервер запущен");
            return true;
        }

        public async Task StopAsync()
        {
            lock (_asyncLocker)
            {
                if (!IsRun) return;
                IsRun = false;
            }
            await server.StopAsync().ConfigureAwait(false);
            logger.Info("Сервер остановлен");
        }
    }
}
