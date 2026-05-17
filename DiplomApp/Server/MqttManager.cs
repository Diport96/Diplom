using DiplomApp.Server.RequsestHandlers;
using MQTTnet.Client;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DiplomApp.Server
{
    class MqttManager : IMqttProtocolManager
    {
        #region Поля и свойства

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static IMqttProtocolManager? instance;
        private readonly IMqttComponent server;
        private readonly IMqttClientComponent client;
        private readonly IRequestHandleManager handleManager;
        private readonly object _asyncLocker;
        private bool isRun;

        public static IMqttProtocolManager Instance
        {
            get
            {
                instance ??= new MqttManager();
                return instance;
            }
        }

        public bool IsRun
        {
            get => isRun;
            private set
            {
                isRun = value;
                OnPropertyChanged("IsRun");
            }
        }

        #endregion

        #region События

        public event EventHandler? MqttProtocolStarted;
        public event EventHandler? MqttProtocolStoped;
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Конструкторы

        private MqttManager()
        {
            server = new MqttServer();
            client = new MqttClient(CallbackAsync);
            handleManager = new RequestHandleManager();
            _asyncLocker = new object();
        }

        internal MqttManager(IMqttComponent server, IMqttClientComponent client) : this()
        {
            this.server = server;
            this.client = client;
        }

        #endregion

        #region Методы запуска/остановки

        public async Task<bool> RunAsync()
        {
            lock (_asyncLocker)
            {
                if (IsRun) return true;
                IsRun = true;
            }
            if (await server.RunAsync() && await client.RunAsync())
            {
                MqttProtocolStarted?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        public async Task StopAsync()
        {
            lock (_asyncLocker)
            {
                if (!IsRun) return;
                IsRun = false;
            }
            if (client.IsRun) await client.StopAsync();
            if (server.IsRun) await server.StopAsync();
            MqttProtocolStoped?.Invoke(this, EventArgs.Empty);
            IsRun = false;
        }

        public void Stop()
        {
            lock (_asyncLocker)
            {
                if (!IsRun) return;
                IsRun = false;
            }
            if (client.IsRun) client.StopAsync().Wait();
            if (server.IsRun) server.StopAsync().Wait();
            MqttProtocolStoped?.Invoke(this, EventArgs.Empty);
            IsRun = false;
        }

        #endregion

        #region Методы отправки сообщений

        public async Task SendMessage(string jsonMessage, string topic)
        {
            await client.SendMessage(jsonMessage, topic).ConfigureAwait(false);
        }

        public async Task SendMessage(Dictionary<string, string> keyValuePairs, string topic)
        {
            await client.SendMessage(keyValuePairs, topic).ConfigureAwait(false);
        }

        #endregion

        #region Методы обработки полученных сообщений

        private Task CallbackAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            var jsonMessage = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            var message = GetDataFromJson(jsonMessage);

            message.TryGetValue("Message_Type", out string? req);
            logger.Trace($"Получено сообщение из топика {e.ApplicationMessage.Topic}. Тип сообщения: {req}");
            if (req == SetOfConstants.MessageTypes.PERMIT_TO_CONNECT) return Task.CompletedTask;

            message.Add("Topic", e.ApplicationMessage.Topic);
            HandleRequest(message);
            return Task.CompletedTask;
        }

        private Dictionary<string, string> GetDataFromJson(string jsonMessage)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonMessage);
                return message ?? new Dictionary<string, string>();
            }
            catch (Exception w)
            {
                logger.Error(w, "Не удалось распарсить данные, возможно нарушение структуры данных");
                throw;
            }
        }

        private void HandleRequest(Dictionary<string, string> message)
        {
            try
            {
                var handler = handleManager.GetRequestHandler(message);
                message.Remove("Message_Type");
                handler.Run(message);
            }
            catch (HandlerNotFindException w)
            {
                logger.Error(w.Message);
            }
        }

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
