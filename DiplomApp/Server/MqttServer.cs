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
    class MqttServer : IMqttComponent
    {
        private readonly IMqttServer server;
        private readonly IMqttServerOptions serverOptions;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly object _asyncLocker;

        public bool IsRun { get; private set; }

        public MqttServer()
        {
            IsRun = false;
            _asyncLocker = new object();
            var mqttFactory = new MqttFactory();
            server = mqttFactory.CreateMqttServer();
            serverOptions = new MqttServerOptions();
        }

        /// <summary>
        /// Конструктор для тестирования
        /// </summary>
        /// <param name="server"></param>
        /// <param name="serverOptions"></param>
        internal MqttServer(IMqttServer server, IMqttServerOptions serverOptions) : this()
        {
            this.server = server;
            this.serverOptions = serverOptions;
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
