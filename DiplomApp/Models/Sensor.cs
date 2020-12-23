using DiplomApp.Controllers;

namespace DiplomApp.Models
{
    class Sensor : Controller
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

        public Sensor(string id, string name, double value) : base(id, name)
        {
            this.value = value;
        }
    }
}
