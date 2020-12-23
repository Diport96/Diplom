using DiplomApp.Controllers;

namespace DiplomApp.Models
{
    internal class Sensor : Controller
    {
        private double value;
        public double Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        public Sensor(string id, string name, double value) : base(id, name)
        {
            this.value = value;
        }
    }
}
