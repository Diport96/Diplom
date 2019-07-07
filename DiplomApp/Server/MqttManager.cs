using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiplomApp.Server.RequsestHandlers;
using MQTTnet;
using Newtonsoft.Json;
using NLog;

namespace DiplomApp.Server
{
    class MqttManager : IMqttProtocolManagaer
    {        
        private static IMqttProtocolManagaer instance;
        private readonly IMqttComponent server;
        private readonly IMqttClientComponent client;
        private readonly object _asyncLocker;

        public static IMqttProtocolManagaer Instance
        {
            get
            {
                if (instance == null) instance = new MqttManager();
                return instance;
            }
        }
        public bool IsRun { get; private set; }

        public event EventHandler MqttProtocolStarted;
        public event EventHandler MqttProtocolStoped;

        private MqttManager()
        {
            server = new MqttServer();
            client = new MqttClient();
            _asyncLocker = new object();
        }

        /// <summary>
        /// Конструктор для тестирования
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        internal MqttManager(IMqttComponent server, IMqttClientComponent client) : this()
        {
            this.server = server;
            this.client = client;
        }

        public async Task<bool> RunAsync()
        {
            lock (_asyncLocker)
            {
                if (IsRun) return true;
                IsRun = true;
            }
            if (await server.RunAsync() && await client.RunAsync())
            {
                MqttProtocolStarted?.Invoke(this, new EventArgs());
                return true;
            }
            IsRun = true;
            return false;
        }
        public async Task StopAsync()
        {
            lock (_asyncLocker)
            {
                if (!IsRun) return;
                IsRun = false;
            }
            if (client.IsRun) await client.StopAsync()
                    .ConfigureAwait(false);
            if (server.IsRun) await server.StopAsync()
                    .ConfigureAwait(false);
            MqttProtocolStoped?.Invoke(this, new EventArgs());
            IsRun = false;
        }

        public async Task SendMessage(string jsonMessage, string topic)
        {
            await client.SendMessage(jsonMessage, topic);
        }
        public async Task SendMessage(Dictionary<string, string> keyValuePairs, string topic)
        {
            await client.SendMessage(keyValuePairs, topic);
        }        
    }
}
