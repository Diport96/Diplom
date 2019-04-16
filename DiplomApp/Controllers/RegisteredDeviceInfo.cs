using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    class RegisteredDeviceInfo : Controller
    {
        [Required]
        public string DeviceType { get; private set; }

        [Required]
        public DateTime RegisteredDate { get; private set; }

        public bool PermitToConnection { get; set; }

        public RegisteredDeviceInfo(string id, string name, string type, DateTime registeredDate) : base(id, name)
        {
            DeviceType = type;
            RegisteredDate = registeredDate;
            PermitToConnection = true;
        }
    }
}
