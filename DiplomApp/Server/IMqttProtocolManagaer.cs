using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    interface IMqttProtocolManagaer : IMqttClientComponent
    {
        event EventHandler MqttProtocolStarted;
        event EventHandler MqttProtocolStoped;
    }
}
