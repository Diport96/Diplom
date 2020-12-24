using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EmuDevicesUtility
{
    internal abstract class Controller : INotifyPropertyChanged
    {
        private string name;
        protected readonly ClientDevice Client;

        public event PropertyChangedEventHandler PropertyChanged;
        public string ID { get; }
        public string Type { get; }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public Options Options { get; }
        public abstract string Value { get; set; }

        protected Controller(string name, string type)
        {
            Type = type;
            ID = Guid.NewGuid().ToString();
            Name = name;
            Client = new ClientDevice(this);
            Options = new Options();
            Client.SendConnect();
        }

        public void Remove()
        {
            Client.SendDisconnect();
        }
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ~Controller()
        {
            Client.SendDisconnect();
        }
    }
}
