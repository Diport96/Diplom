using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    public class RegisteredDeviceInfo : Controller
    {
        [Required]
        public string DeviceType { get; set; }
        [Required]
        public DateTime RegisteredDate { get; set; }
        public bool PermitToConnection { get; set; }
        public SwitchOptions Options { get; set; }

        public RegisteredDeviceInfo()
        {
            Options = new SwitchOptions();
        }
        public RegisteredDeviceInfo(string id, string name, string type, DateTime registeredDate) : base(id, name)
        {
            DeviceType = type;
            RegisteredDate = registeredDate;
            PermitToConnection = true;
            Options = new SwitchOptions();
        }
    }
}
