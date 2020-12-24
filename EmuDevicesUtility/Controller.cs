using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EmuDevicesUtility;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EmuDevicesUtility
{
    abstract class Controller : INotifyPropertyChanged
    {
        string name;
        protected readonly ClientDevice Client;

        public event PropertyChangedEventHandler PropertyChanged;
        public string ID { get; }
        public string Type { get; }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public Options Options { get; }
        abstract public string Value { get; set; }

        public Controller(string name, string type)
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
