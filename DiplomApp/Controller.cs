using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTTestApp
{
    abstract class Controller : INotifyPropertyChanged
    {
        string name;
        readonly ClientDevice Client;

        public event PropertyChangedEventHandler PropertyChanged;
        public string ID { get; }
        public CType Type { get; }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        abstract public string Value { get; set; }

        public Controller(string name)
        {
            Type = TypeInit();
            ID = Guid.NewGuid().ToString();
            Name = name;
            Client = new ClientDevice(this);

        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        internal abstract CType TypeInit();
    }
}
