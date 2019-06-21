using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.Data;
using DiplomApp.Server;
using DiplomApp.ViewModels.Commands;
using DiplomApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DiplomApp.Controllers.SwitchOptions;

namespace DiplomApp.ViewModels
{
    class SwitchSettingsViewModel : INotifyPropertyChanged
    {
        #region Поля

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Window owner;
        private readonly RegisteredDeviceContext database;
        private readonly RegisteredDeviceInfo deviceInfo;
        private readonly Switch @switch;

        private bool defaultSettings = false;
        private bool switchToDelaySettings = false;
        private bool switchToSignalSettings = false;

        private int switchDelayHours;
        private int switchDelayMinutes;
        private int switchDelaySeconds;

        private string deviceName;
        private string enableDisableSwitchButtonContent;
        private SwitchControl control;
        private Controller selectedSensor;
        private string selectedChangeSwitchValueTo;

        #endregion

        #region Свойства

        public string DeviceName
        {
            get { return deviceName; }
            set
            {
                deviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }
        public string EnableDisableSwitchButtonContent
        {
            get { return enableDisableSwitchButtonContent; }
            set
            {
                enableDisableSwitchButtonContent = value;
                OnPropertyChanged("EnableDisableSwitchButtonContent");
            }
        }
        public Controller SelectedSensor
        {
            get { return selectedSensor; }
            set
            {
                selectedSensor = value;
                OnPropertyChanged("SelectedSensor");
            }
        }
        public Dictionary<string, bool> ChangeSwitchValueTo { get; }
        = new Dictionary<string, bool>()
        {
            { "Включить", true },
            { "Выключить", false }
        };
        public string SelectedChangeSwitchValueTo
        {
            get { return selectedChangeSwitchValueTo; }
            set
            {
                selectedChangeSwitchValueTo = value;
                OnPropertyChanged("SelectedChangeSwitchValueTo");
            }
        }

        public bool DefaultSettings
        {
            get { return defaultSettings; }
            set
            {
                defaultSettings = value;
                if (value) control = SwitchControl.No;
                OnPropertyChanged("DefaultSettings");
            }
        }
        public bool SwitchToDelaySettings
        {
            get { return switchToDelaySettings; }
            set
            {
                switchToDelaySettings = value;
                if (value) control = SwitchControl.SwitchToDelay;
                OnPropertyChanged("SwitchToDelaySettings");
            }
        }
        public bool SwitchToSignalSettings
        {
            get { return switchToSignalSettings; }
            set
            {
                switchToSignalSettings = value;
                if (value) control = SwitchControl.SwitchToSignal;
                OnPropertyChanged("SwitchToSignalSettings");
            }
        }

        public int SwitchDelayHours
        {
            get { return switchDelayHours; }
            set
            {
                switchDelayHours = value;
                OnPropertyChanged("SwitchDelayHours");
            }
        }
        public int SwitchDelayMinutes
        {
            get { return switchDelayMinutes; }
            set
            {
                switchDelayMinutes = value;
                OnPropertyChanged("SwitchDelayMinutes");
            }
        }
        public int SwitchDelaySeconds
        {
            get { return switchDelaySeconds; }
            set
            {
                switchDelaySeconds = value;
                OnPropertyChanged("SwitchDelaySeconds");
            }
        }

        #endregion

        #region Команды

        private AsyncRelayCommand enableDisableSwitchButtonCommand;
        private RelayCommand selectSensorCommand;
        private RelayCommand submitChangesCommand;
        private RelayCommand cancelChangesCommand;

        public AsyncRelayCommand EnableDisableSwitchButtonCommand
        {
            get
            {
                return enableDisableSwitchButtonCommand ??
                    (enableDisableSwitchButtonCommand = new AsyncRelayCommand(obj => EnableDisableSwitchButton(obj as Button)));
            }
        }
        public RelayCommand SelectSensorCommand
        {
            get
            {
                return selectSensorCommand ??
                    (selectSensorCommand = new RelayCommand(obj => SelectSensor()));
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

        #endregion

        public SwitchSettingsViewModel(Window owner, string deviceId)
        {
            this.owner = owner;
            database = new RegisteredDeviceContext();
            deviceInfo = database.RegisteredDevices.First(x => x.ID == deviceId);
            database.SwitchOptions.First(x => x.ID == deviceInfo.ID);

            DeviceName = deviceInfo.Name;
            @switch = ControllersFactory.GetById(deviceId) as Switch;
            control = deviceInfo.Options.Control;
            switch (control)
            {
                case SwitchControl.No:
                    DefaultSettings = true;
                    break;
                case SwitchControl.SwitchToDelay:
                    SwitchToDelaySettings = true;
                    break;
                case SwitchControl.SwitchToSignal:
                    SwitchToDelaySettings = true;
                    break;
                default:
                    break;
            }

            SetEnableDisableSwitchButtonState(@switch.Value);
            if (deviceInfo.Options.DelayToSwitch.HasValue)
            {
                var time = TimeSpan.FromMilliseconds(deviceInfo.Options.DelayToSwitch.Value);
                SwitchDelayHours = time.Hours;
                SwitchDelayMinutes = time.Minutes;
                SwitchDelaySeconds = time.Seconds;
            }
            else
            {
                SwitchDelayHours = 0;
                SwitchDelayMinutes = 0;
                SwitchDelaySeconds = 0;
            }
            if (deviceInfo.Options.SensorId != null)
            {
                SelectedSensor = ControllersFactory.GetById(deviceInfo.Options.SensorId);
            }
            else
            {
                selectedSensor = null;
            }
            SelectedChangeSwitchValueTo = ChangeSwitchValueTo.First().Key;
        }

        #region Методы

        public void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async Task EnableDisableSwitchButton(Button button)
        {
            button.IsEnabled = false;
            @switch.Value = !@switch.Value;
            SetEnableDisableSwitchButtonState(@switch.Value);
            var response = ResponseManager.SetSwitchStateToDictionary(@switch.ID, @switch.Value);
            await App.Server.SendMessage(response, Server.SetOfConstants.Topics.SWITCHES);
            button.IsEnabled = true;
        }
        private void SetEnableDisableSwitchButtonState(bool value)
        {
            if (value)
                EnableDisableSwitchButtonContent = "Выключить";
            else
                EnableDisableSwitchButtonContent = "Включить";
        }
        private void SelectSensor()
        {
            var dialogWindow = new SelectSensorDialogWindow();
            if (dialogWindow.ShowDialog().Value)
                SelectedSensor = (dialogWindow.DataContext as SelectSensorDialogViewModel).SelectedSensor;
        }
        private void SubmitChanges()
        {
            deviceInfo.Name = DeviceName;
            @switch.Name = DeviceName;

            deviceInfo.Options.Control = control;
            deviceInfo.Options.DelayToSwitch = (int)new TimeSpan(SwitchDelayHours, SwitchDelayMinutes, SwitchDelaySeconds).TotalMilliseconds;
            if (SelectedSensor != null) deviceInfo.Options.SensorId = selectedSensor.ID;
            else deviceInfo.Options.SensorId = default;
            deviceInfo.Options.ValueTo = ChangeSwitchValueTo[SelectedChangeSwitchValueTo];

            database.SaveChanges();
            var response = ResponseManager.SetSwitchOptionsToDictionary(@switch.ID, deviceInfo.Options);
            App.Server.SendMessage(response, Server.SetOfConstants.Topics.SWITCHES).GetAwaiter().GetResult();
            owner.DialogResult = true;
        }
        private void CancelChanges()
        {
            owner.DialogResult = false;
        }

        #endregion
    }
}
