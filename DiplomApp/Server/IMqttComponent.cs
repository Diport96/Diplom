using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    interface IMqttComponent
    {
        bool IsRun { get; }

        Task<bool> RunAsync();
        Task StopAsync();
    }
}
