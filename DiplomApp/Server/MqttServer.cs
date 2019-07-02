using MQTTnet;
using MQTTnet.Server;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    class MqttServer
    {
        private static MqttServer instance;
        private readonly IMqttServer server;
        private readonly IMqttServerOptions serverOptions;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly object _asyncLocker;

        public static MqttServer Instance
        {
            get
            {
                if (instance == null) instance = new MqttServer();
                return instance;
            }
        }
        public bool IsRun { get; private set; }

        private MqttServer()
        {
            IsRun = false;
            _asyncLocker = new object();
            var mqttFactory = new MqttFactory();
            server = mqttFactory.CreateMqttServer();
            serverOptions = new MqttServerOptions();
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
                await server.StartAsync(serverOptions);
            }
            catch (InvalidOperationException e)
            {
                logger.Error(e, e.Message);
                return false;
            }
            IsRun = true;
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

            await server.StopAsync()
                .ConfigureAwait(false);
            logger.Info("Сервер остановлен");
        }
    }
}
