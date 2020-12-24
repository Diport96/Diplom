using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDevicesUtility
{
    class Switch : Controller
    {
        private bool value;
        public override string Value
        {
            get { return value.ToString(); }
            set
            {
                this.value = bool.Parse(value);
                OnPropertyChanged("Value");
            }
        }
        public Switch(string name, bool value) : base(name, "Switch")
        {
            this.value = value;
            PropertyChanged += Switch_PropertyChanged;
        }

        private void Switch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Client.DistributeValue();
            }            
        }

        public void SetValue(bool val)
        {
            value = val;
            OnPropertyChanged("Value");
        }
    }
}
