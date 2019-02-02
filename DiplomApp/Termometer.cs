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
    class Termometer : Controller
    {
        private int value;
        public override string Value
        {
            get { return value.ToString(); }
            set
            {
                this.value = int.Parse(value);
                OnPropertyChanged("Value");
            }
        }
        public Termometer(string name, int value) : base(name)
        {
            this.value = value;            
        }
        public void SetValue(int val)
        {
            value = val;
            OnPropertyChanged("Value");
        }
        internal override CType TypeInit()
        {
            return CType.Termometer;
        }
    }
}
