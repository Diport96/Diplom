using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DiplomApp
{
    class Termometer : Controller
    {
        private double value;
        public double Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        public Termometer(Guid id, string name, double value) : base(id, name)
        {
            this.value = value;            
        }        

        protected override CType TypeInit()
        {
            return CType.Termometer;
        }
    }
}
