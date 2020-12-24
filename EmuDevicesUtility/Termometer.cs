using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDevicesUtility
{
    class Termometer : Controller
    {
        private double value;
        public override string Value
        {
            get { return value.ToString(); }
            set
            {
                this.value = double.Parse(value);
                OnPropertyChanged("Value");
            }
        }

        public Termometer(string name, double value) : base(name, "Thermometer")
        {
            this.value = value;
            Client.AfterSendMessage += ClientAfterSendMessage;
        }

        private void ClientAfterSendMessage(object sender, EventArgs e)
        {
            var x = DateTime.Now.Millisecond;
            var res = Math.Abs(Math.Sin(x * 20) * 20);
            SetValue(res);
        }

        public void SetValue(double val)
        {
            Value = val.ToString();
        }
    }
}
