using DiplomApp.Accounts;
using DiplomApp.Controllers;
using DiplomApp.Server;
using DiplomApp.ViewModels.Services;
using NLog;
using System.Windows;

namespace DiplomApp
{
    public partial class App : Application
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        internal static IMqttProtocolManager Server { get; internal set; } = null!;
        internal static IAccountManager UserAccountManager { get; internal set; } = null!;
        internal static IControllersFactory ControllersFactory { get; internal set; } = null!;
        internal static IWindowService WindowService { get; internal set; } = null!;

        App()
        {
            InitializeComponent();
            Exit += App_Exit;
            Startup += App_Startup;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            SetLogSettings();

            Server = MqttManager.Instance;
            UserAccountManager = AccountManager.Instance;
            ControllersFactory = Controllers.ControllersFactory.Instance;
            WindowService = new WindowService();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal(e.ExceptionObject.ToString());
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            logger.Info("Запуск приложения");
            WindowService.OpenAuthenticationWindow();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            Server.Stop();
            logger.Info("Завершение работы приложения");
        }

        private static void SetLogSettings()
        {
            if (Properties.Settings.Default.EnableDebugInfo)
                LogManager.Configuration?.LoggingRules[0].EnableLoggingForLevels(LogLevel.Trace, LogLevel.Debug);
            else
                LogManager.Configuration?.LoggingRules[0].DisableLoggingForLevels(LogLevel.Trace, LogLevel.Debug);
        }
    }
}
