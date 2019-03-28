using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    class Switch : Controller
    {
        private bool value;
        [NotMapped]
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
