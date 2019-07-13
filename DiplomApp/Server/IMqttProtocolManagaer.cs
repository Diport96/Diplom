using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    interface IMqttProtocolManager : IMqttClientComponent, INotifyPropertyChanged
    {
        event EventHandler MqttProtocolStarted;
        event EventHandler MqttProtocolStoped;
    }
}
