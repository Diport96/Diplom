using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace DiplomApp.Properties
{
    internal sealed class Settings
    {
        private static readonly Settings _default = Load();
        private static readonly string _userSettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DiplomApp", "usersettings.json");

        public static Settings Default => _default;

        // Application-scoped (from appsettings.json, read-only)
        public string ServerDomain { get; private set; } = "localhost";
        public string MongoDbConnectionString { get; private set; } = "mongodb://localhost/DevicesData";
        public string SqlConnectionString { get; private set; } =
            "Server=(localdb)\\MSSQLLocalDB;Database=DeviceDatabase;Trusted_Connection=True;";

        // User-scoped (persisted to AppData, read-write)
        public string WebAppUrl { get; set; } = "https://localhost:44381";
        public bool AutoSendDataToWebApp { get; set; } = false;
        public bool EnableDebugInfo { get; set; } = true;
        public TimeSpan AutoSendDataEvery { get; set; } = TimeSpan.FromHours(20);

        private static Settings Load()
        {
            var s = new Settings();

            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();

                s.ServerDomain = config["AppSettings:ServerDomain"] ?? s.ServerDomain;
                s.MongoDbConnectionString = config.GetConnectionString("MongoDb") ?? s.MongoDbConnectionString;
                s.SqlConnectionString = config.GetConnectionString("SqlDatabase") ?? s.SqlConnectionString;
            }
            catch { }

            try
            {
                if (File.Exists(_userSettingsPath))
                {
                    var json = File.ReadAllText(_userSettingsPath);
                    var u = JsonSerializer.Deserialize<UserSettings>(json);
                    if (u != null)
                    {
                        s.WebAppUrl = u.WebAppUrl;
                        s.AutoSendDataToWebApp = u.AutoSendDataToWebApp;
                        s.EnableDebugInfo = u.EnableDebugInfo;
                        s.AutoSendDataEvery = u.AutoSendDataEvery;
                    }
                }
            }
            catch { }

            return s;
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_userSettingsPath)!);
                var u = new UserSettings
                {
                    WebAppUrl = WebAppUrl,
                    AutoSendDataToWebApp = AutoSendDataToWebApp,
                    EnableDebugInfo = EnableDebugInfo,
                    AutoSendDataEvery = AutoSendDataEvery
                };
                var json = JsonSerializer.Serialize(u, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_userSettingsPath, json);
            }
            catch { }
        }

        private sealed class UserSettings
        {
            public string WebAppUrl { get; set; } = "https://localhost:44381";
            public bool AutoSendDataToWebApp { get; set; }
            public bool EnableDebugInfo { get; set; } = true;
            public TimeSpan AutoSendDataEvery { get; set; } = TimeSpan.FromHours(20);
        }
    }
}
