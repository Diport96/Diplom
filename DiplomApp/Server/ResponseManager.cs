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
                { "ID", id }
            };

            return res;
        }
        public static Dictionary<string, string> ChangeSwitchStateToDictionary(string id, bool value)
        {
            var res = new Dictionary<string, string>
            {
                { "Message_Type", MessageTypes.CHANGE_SWITCH_STATE },
                { "ID", id },
                { "Value", value.ToString() }
            };

            return res;
        }
    }
}
