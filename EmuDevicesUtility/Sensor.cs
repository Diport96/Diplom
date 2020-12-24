namespace EmuDevicesUtility
{
    internal class Sensor : Controller
    {
        private int value;
        public override string Value
        {
            get => value.ToString();
            set
            {
                this.value = int.Parse(value);
                OnPropertyChanged();
            }
        }
       
        public Sensor(string name, int value) : base(name, "Sensor")
        {
            this.value = value;
        }

        public void SetValue(int val)
        {
            Value = val.ToString();
        }
    }
}
