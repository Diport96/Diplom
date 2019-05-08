using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace DiplomApp.Controllers.Models
{
    class Termometer : Sensor
    {


        public Termometer(string id, string name, double value) : base(id, name, value)
        {

        }
    }
}
