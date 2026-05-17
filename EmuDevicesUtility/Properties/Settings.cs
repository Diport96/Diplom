namespace EmuDevicesUtility.Properties
{
    internal sealed class Settings
    {
        private static readonly Settings _default = new Settings();
        public static Settings Default => _default;

        public TimeSpan SendMessageDelay { get; set; } = TimeSpan.FromSeconds(3);
    }
}
