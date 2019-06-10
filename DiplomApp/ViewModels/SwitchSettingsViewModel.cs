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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DiplomApp.Controllers.SwitchOptions;

namespace DiplomApp.ViewModels
{
    class SwitchSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Window owner;
        private readonly RegisteredDeviceContext database;
        private readonly RegisteredDeviceInfo deviceInfo;
        private readonly Switch @switch;
        private AsyncRelayCommand enableDisableSwitchButtonCommand;
        private RelayCommand selectSensorCommand;

        private string deviceName;
        private string enableDisableSwitchButtonContent;
        private bool defaultSettings = false;
        private bool switchToDelaySettings = false;
        private bool switchToSignalSettings = false;
        private SwitchControl control;
        private Controller selectedSensor;

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
                OnPropertyChanged("EnableDisableButtonContent");
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
        public Controller SelectedSensor
        {
            get { return selectedSensor; }
            set
            {
                selectedSensor = value;
                OnPropertyChanged("SelectedSensor");
            }
        }
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
        }


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
                if (dialogWindow.Answer != null)
                    selectedSensor = dialogWindow.Answer;
        }

        // Next
        private void SubmitChanges()
        {
            deviceInfo.Name = DeviceName;
            database.SaveChanges();
            @switch.Name = DeviceName;
            database.SwitchOptions.First(x => x.ID == deviceInfo.ID);

            deviceInfo.Options.Control = control;
           

            owner.DialogResult = true;
        }
    }
}
