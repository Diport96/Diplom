using DiplomApp.Controllers;
using DiplomApp.Data;
using DiplomApp.ViewModels.Commands;
using System.Linq;
using System.Windows;

namespace DiplomApp.ViewModels
{
    class SensorSettingsViewModel : BaseViewModel
    {
        private readonly Window owner;
        private string deviceName;
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

        public SensorSettingsViewModel(Window owner, string deviceId)
        {
            this.owner = owner;
            database = new RegisteredDeviceContext();
            deviceInfo = database.RegisteredDevices.First(x => x.ID == deviceId);
            DeviceName = deviceInfo.Name;
        }

        private void SubmitChanges()
        {
            deviceInfo.Name = DeviceName;
            database.SaveChanges();
            ControllersFactory.GetById(deviceInfo.ID).Name = DeviceName;

            owner.DialogResult = true;
        }
        private void CancelChanges()
        {
            owner.DialogResult = false;
        }
    }
}
