using System;

namespace EmuDevicesUtility
{
    [Flags]
    internal enum SwitchControl
    {
        No,
        SwitchToDelay,
        SwitchToSignal
    };
}
