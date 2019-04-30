using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    public class SwitchOptions
    {
        [Key]
        [ForeignKey("DeviceInfo")]
        public string ID { get; set; }
        [Flags]
        public enum SwitchControl
        {
            No,
            SwitchToDelay,
            SwitchToSignal
        };
        public SwitchControl Control { get; set; }
        public string SensorId { get; set; }
        public int? DelayToSwitch { get; set; }
        public bool? ValueTo { get; set; }
        public RegisteredDeviceInfo DeviceInfo { get; set; }

        public SwitchOptions()
        {
            ID = new Guid().ToString();
            Control = SwitchControl.No;
            SensorId = null;
            DelayToSwitch = null;
            ValueTo = null;
        }
        public SwitchOptions(int delay, bool valueTo)
        {
            Control = SwitchControl.SwitchToDelay;
            SensorId = null;
            DelayToSwitch = delay;
            ValueTo = valueTo;
        }
        public SwitchOptions(string sensorId, bool valueTo)
        {
            Control = SwitchControl.SwitchToSignal;
            SensorId = sensorId;
            DelayToSwitch = null;
            ValueTo = valueTo;
        }
    }
}
