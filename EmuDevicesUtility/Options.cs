namespace EmuDevicesUtility
{
    internal class Options
    {
        public string SensorId { get; set; }
        public int? DelayToSwitch { get; set; }
        public SwitchControl Control { get; set; }
        public bool? ValueTo { get; set; }

        public void SetOptions()
        {
            Control = SwitchControl.No;
            SensorId = null;
            DelayToSwitch = null;
            ValueTo = null;
        }
        public void SetOptions(int delay, bool valueTo)
        {
            Control = SwitchControl.SwitchToDelay;
            SensorId = null;
            DelayToSwitch = delay;
            ValueTo = valueTo;
        }
        public void SetOptions(string sensorId, bool valueTo)
        {
            Control = SwitchControl.SwitchToSignal;
            SensorId = sensorId;
            DelayToSwitch = null;
            ValueTo = valueTo;
        }

    }
}
