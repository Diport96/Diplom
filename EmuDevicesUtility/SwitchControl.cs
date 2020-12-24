using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDevicesUtility
{
    [Flags]
    enum SwitchControl
    {
        No,
        SwitchToDelay,
        SwitchToSignal
    };
}
