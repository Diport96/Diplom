using DiplomApp.Controllers;
using DiplomApp.Data;
using DiplomApp.ViewModels.Commands;
using System;
using System.Linq;
using System.Windows;

namespace DiplomApp.ViewModels
{
    class SensorSettingsViewModel : DialogBaseViewModel
    {
        private string deviceName;
        private readonly RegisteredDeviceContext database;
        private readonly RegisteredDeviceInfo deviceInfo;

        public string DeviceName
        {
            get { return deviceName; }
            set
            {
                deviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }

        public SensorSettingsViewModel(string deviceId, Action<bool> dialogResultWindow)
            : base(dialogResultWindow)
        {
            database = new RegisteredDeviceContext();
            deviceInfo = database.RegisteredDevices.First(x => x.ID == deviceId);
            DeviceName = deviceInfo.Name;
        }

        protected override void Submit()
        {
            deviceInfo.Name = DeviceName;
            database.SaveChanges();
            App.ControllersFactory.GetById(deviceInfo.ID).Name = DeviceName;

            dialogResultWindowAction(true);
        }
        protected override void Cancel()
        {
            dialogResultWindowAction(false);
        }
    }
}
