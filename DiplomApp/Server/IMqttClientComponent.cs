using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    interface IMqttClientComponent : IMqttComponent
    {
        Task SendMessage(string jsonMessage, string topic);
        Task SendMessage(Dictionary<string, string> keyValuePairs, string topic);
    }
}
