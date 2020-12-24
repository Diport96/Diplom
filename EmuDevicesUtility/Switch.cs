namespace EmuDevicesUtility
{
    internal class Switch : Controller
    {
        private bool value;
        public override string Value
        {
            get => value.ToString();
            set
            {
                this.value = bool.Parse(value);
                OnPropertyChanged();
            }
        }
        public Switch(string name, bool value) : base(name, "Switch")
        {
            this.value = value;
            PropertyChanged += Switch_PropertyChanged;
        }

        private void Switch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Client.DistributeValue();
            }            
        }

        public void SetValue(bool val)
        {
            value = val;
            OnPropertyChanged(nameof(Value));
        }
    }
}
