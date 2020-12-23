using DiplomApp.Controllers;

namespace DiplomApp.Models
{
    class Switch : Controller
    {
        private bool value;       
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        public Switch(string id, string name, bool value) : base(id, name)
        {
            Value = value;
        }
    }
}
