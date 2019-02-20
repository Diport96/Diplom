using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
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
