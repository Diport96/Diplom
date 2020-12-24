using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDevicesUtility
{
    class Sensor : Controller
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
       
        public Sensor(string name, int value) : base(name, "Sensor")
        {
            this.value = value;
        }

        public void SetValue(int val)
        {
            Value = val.ToString();
        }
    }
}
