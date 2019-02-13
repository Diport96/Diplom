using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


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

        public Termometer(string id, string name, double value) : base(id, name)
        {
            this.value = value;
        }
    }
}
