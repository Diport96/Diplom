using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace DiplomApp.Controllers
{
    class Termometer : Controller
    {
        private double value;
        [NotMapped]
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
