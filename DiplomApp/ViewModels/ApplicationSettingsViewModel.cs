using DiplomApp.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DiplomApp.ViewModels
{
    class ApplicationSettingsViewModel : BaseViewModel
    {
        private readonly Window owner;
        private bool autoSendData;
        private bool enableDebugInfo;
        private string webAppUrl;
        private string selectedAutoSendDataTime;
        private RelayCommand submitChangesCommand;
        private RelayCommand cancelChangesCommand;

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
            get { return autoSendData; }
            set
            {
                autoSendData = value;
                OnPropertyChanged("AutoSendData");
            }
        }
        public string SelectedAutoSendDataTime
        {
            get { return selectedAutoSendDataTime; }
            set
            {
                selectedAutoSendDataTime = value;
                OnPropertyChanged("SelectedAutoSendDataTime");
            }
        }
        public bool EnableDebugInfo
        {
            get { return enableDebugInfo; }
            set
            {
                enableDebugInfo = value;
                OnPropertyChanged("EnableDebugInfo");
            }
        }
        public string WebAppUrl
        {
            get { return webAppUrl; }
            set
            {
                webAppUrl = value;
                OnPropertyChanged("ServerUrl");
            }
        }
        public RelayCommand SubmitChangesCommand
        {
            get
            {
                return submitChangesCommand ??
                    (submitChangesCommand = new RelayCommand(obj => SubmitChanges()));
            }
        }
        public RelayCommand CancelChangesCommand
        {
            get
            {
                return cancelChangesCommand ??
                    (cancelChangesCommand = new RelayCommand(obj => CancelChanges()));
            }
        }

        public ApplicationSettingsViewModel(Window owner)
        {
            this.owner = owner;
            WebAppUrl = Properties.Settings.Default.WebAppUrl;
            AutoSendData = Properties.Settings.Default.AutoSendDataToWebApp;
            EnableDebugInfo = Properties.Settings.Default.EnableDebugInfo;
        }

        private void SubmitChanges()
        {
            Properties.Settings.Default.WebAppUrl = WebAppUrl;
            Properties.Settings.Default.AutoSendDataToWebApp = AutoSendData;
            if (!string.IsNullOrEmpty(SelectedAutoSendDataTime)) Properties.Settings.Default.AutoSendDataEvery = AutoSendDataEvery[SelectedAutoSendDataTime];
            Properties.Settings.Default.EnableDebugInfo = EnableDebugInfo;

            Properties.Settings.Default.Save();
            owner.DialogResult = true;
        }
        private void CancelChanges()
        {
            owner.DialogResult = false;
        }
    }
}
