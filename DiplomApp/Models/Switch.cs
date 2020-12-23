using DiplomApp.Controllers;

namespace DiplomApp.Models
{
    internal class Switch : Controller
    {
        private bool value;       
        public bool Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        public Switch(string id, string name, bool value) : base(id, name)
        {
            Value = value;
        }
    }
}
