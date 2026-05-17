using DiplomApp.ViewModels.Commands;

namespace DiplomApp.ViewModels
{
    class ApplicationSettingsViewModel : DialogBaseViewModel
    {
        private bool autoSendData;
        private bool enableDebugInfo;
        private string webAppUrl = string.Empty;
        private string? selectedAutoSendDataTime;

        public Dictionary<string, TimeSpan> AutoSendDataEvery { get; }
            = new Dictionary<string, TimeSpan>()
        {
            { "30 минут", new TimeSpan(0, 30, 0) },
            { "1 час", new TimeSpan(1, 0, 0) },
            { "2 часа", new TimeSpan(2, 0, 0) },
            { "5 часов", new TimeSpan(5, 0, 0) },
            { "16 часов", new TimeSpan(16, 0, 0) },
            { "1 день", new TimeSpan(1, 0, 0, 0) },
            { "3 дня", new TimeSpan(3, 0, 0, 0) },
            { "1 неделя", new TimeSpan(7, 0, 0, 0) }
        };

        public bool AutoSendData
        {
            get => autoSendData;
            set
            {
                autoSendData = value;
                OnPropertyChanged("AutoSendData");
            }
        }

        public string? SelectedAutoSendDataTime
        {
            get => selectedAutoSendDataTime;
            set
            {
                selectedAutoSendDataTime = value;
                OnPropertyChanged("SelectedAutoSendDataTime");
            }
        }

        public bool EnableDebugInfo
        {
            get => enableDebugInfo;
            set
            {
                enableDebugInfo = value;
                OnPropertyChanged("EnableDebugInfo");
            }
        }

        public string WebAppUrl
        {
            get => webAppUrl;
            set
            {
                webAppUrl = value;
                OnPropertyChanged("ServerUrl");
            }
        }

        public ApplicationSettingsViewModel(Action<bool> dialogResultWindow)
            : base(dialogResultWindow)
        {
            var settings = Properties.Settings.Default;
            WebAppUrl = settings.WebAppUrl;
            AutoSendData = settings.AutoSendDataToWebApp;
            EnableDebugInfo = settings.EnableDebugInfo;
        }

        protected override void Submit()
        {
            var settings = Properties.Settings.Default;
            settings.WebAppUrl = WebAppUrl;
            settings.AutoSendDataToWebApp = AutoSendData;
            if (!string.IsNullOrEmpty(SelectedAutoSendDataTime))
                settings.AutoSendDataEvery = AutoSendDataEvery[SelectedAutoSendDataTime];
            settings.EnableDebugInfo = EnableDebugInfo;
            settings.Save();
            dialogResultWindowAction(true);
        }

        protected override void Cancel()
        {
            dialogResultWindowAction(false);
        }
    }
}
