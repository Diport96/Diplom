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
        public override string Value
        {
            get { return value.ToString(); }
            set
            {
                if (!double.TryParse(value, out double tmp))
                    throw new ArgumentException("Не удалось распарсить передаваемое значение");
                this.value = tmp;
                OnPropertyChanged("Value");
            }
        }

        public Termometer(Guid id, string name, double value) : base(id, name)
        {
            this.value = value;            
        }        

        internal override CType TypeInit()
        {
            return CType.Termometer;
        }
    }
}
