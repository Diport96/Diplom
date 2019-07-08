﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DiplomApp.Server.RequsestHandlers;
using MQTTnet;
using Newtonsoft.Json;
using NLog;

namespace DiplomApp.Server
{
    class MqttManager : IMqttProtocolManagaer, INotifyPropertyChanged
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static IMqttProtocolManagaer instance;
        private readonly IMqttComponent server;
        private readonly IMqttClientComponent client;
        private readonly object _asyncLocker;
        private bool isRun;

        public static IMqttProtocolManagaer Instance
        {
            get
            {
                if (instance == null) instance = new MqttManager();
                return instance;
            }
        }
        public bool IsRun
        {
            get { return isRun; }
            private set
            {
                isRun = value;
                OnPropertyChanged("IsRun");
            }
        }

        public event EventHandler MqttProtocolStarted;
        public event EventHandler MqttProtocolStoped;
        public event PropertyChangedEventHandler PropertyChanged;

        private MqttManager()
        {
            server = new MqttServer();
            client = new MqttClient(Callback);
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

        protected void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private void Callback(object sender, MqttApplicationMessageReceivedEventArgs e)
        {

            var jsonMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var message = GetDataFromJson(jsonMessage);

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
        private Dictionary<string, string> GetDataFromJson(string jsonMessage)
        {
            try
            {
                var message = JsonConvert.DeserializeObject(jsonMessage, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
                return message;
            }
            catch (Exception w)
            {
                logger.Error(w, "Не удалось распарсить данные, возможно нарушение структуры данных");
                throw;
            }
        }
    }
}
