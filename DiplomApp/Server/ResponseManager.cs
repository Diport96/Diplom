using SetOfConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    static class ResponseManager
    {
        public static Dictionary<string, string> ConnackToDictionary(string id)
        {
            var res = new Dictionary<string, string>
            {
                { "Message_Type", MessageTypes.PERMIT_TO_CONNECT },
                { "ID", id },
            };
            var regOptions = Controllers.ControllersFactory.GetControllerInfo(id).Options;
            if (regOptions != null)
            {
                var control = regOptions.Control;
                res.Add("Control", control.ToString());

                switch (control)
                {
                    case Controllers.SwitchOptions.SwitchControl.No:
                        break;
                    case Controllers.SwitchOptions.SwitchControl.SwitchToDelay:
                        res.Add("Delay", regOptions.DelayToSwitch.Value.ToString());
                        res.Add("ValueTo", regOptions.ValueTo.Value.ToString());
                        break;
                    case Controllers.SwitchOptions.SwitchControl.SwitchToSignal:
                        res.Add("SensorId", regOptions.SensorId);
                        res.Add("ValueTo", regOptions.ValueTo.Value.ToString());
                        break;
                    default:
                        break;
                }
            }

            return res;
        }
        public static Dictionary<string, string> SetSwitchStateToDictionary(string id, bool value)
        {
            var res = new Dictionary<string, string>
            {
                { "Message_Type", MessageTypes.CHANGE_SWITCH_STATE },
                { "ID", id },
                { "Value", value.ToString() }
            };

            return res;
        }
        public static Dictionary<string, string> SetSwitchOptionsToDictionary(string id, Controllers.SwitchOptions newOptions)
        {
            var res = new Dictionary<string, string>
            {
                { "Message_Type", MessageTypes.SET_NEW_SWITCH_OPTIONS },
                { "ID", id },
                {"Control", newOptions.Control.ToString() }
            };

            switch (newOptions.Control)
            {
                case Controllers.SwitchOptions.SwitchControl.No:
                    break;
                case Controllers.SwitchOptions.SwitchControl.SwitchToDelay:
                    res.Add("Delay", newOptions.DelayToSwitch.Value.ToString());
                    res.Add("ValueTo", newOptions.ValueTo.Value.ToString());
                    break;
                case Controllers.SwitchOptions.SwitchControl.SwitchToSignal:
                    res.Add("SensorId", newOptions.SensorId);
                    res.Add("ValueTo", newOptions.ValueTo.Value.ToString());
                    break;
                default:
                    break;
            }

            return res;
        }
    }
}
