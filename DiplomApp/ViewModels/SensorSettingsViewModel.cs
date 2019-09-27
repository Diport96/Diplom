using DiplomApp.Controllers;
using DiplomApp.Data;
using DiplomApp.ViewModels.Commands;
using System;
using System.Linq;
using System.Windows;

namespace DiplomApp.ViewModels
{
    class SensorSettingsViewModel : BaseViewModel
    {
        private string deviceName;
        private readonly Action<bool> dialogResultWindowAction;
        private readonly RegisteredDeviceContext database;
        private readonly RegisteredDeviceInfo deviceInfo;
        private RelayCommand submitChangesCommand;
        private RelayCommand cancelChangesCommand;

        public string DeviceName
        {
            get { return deviceName; }
            set
            {
                deviceName = value;
                OnPropertyChanged("DeviceName");
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

        public SensorSettingsViewModel(string deviceId, Action<bool> dialogResultWindow)
        {
            database = new RegisteredDeviceContext();
            deviceInfo = database.RegisteredDevices.First(x => x.ID == deviceId);
            dialogResultWindowAction = dialogResultWindow;
            DeviceName = deviceInfo.Name;
        }

        private void SubmitChanges()
        {
            deviceInfo.Name = DeviceName;
            database.SaveChanges();
            App.ControllersFactory.GetById(deviceInfo.ID).Name = DeviceName;

            dialogResultWindowAction(true);
        }
        private void CancelChanges()
        {
            dialogResultWindowAction(false);
        }
    }
}
