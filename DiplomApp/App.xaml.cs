using DiplomApp.Accounts;
using DiplomApp.Controllers;
using DiplomApp.Server;
using DiplomApp.ViewModels.Services;
using NLog;
using System;
using System.Windows;

namespace DiplomApp
{
    public partial class App : Application
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Является экземпляром сервера, доступным на уровне всего приложения
        /// </summary>
        internal static IMqttProtocolManager Server { get; private set; }
        internal static IAccountManager UserAccountManager { get; private set; }
        internal static IControllersFactory ControllersFactory { get; private set; }
        internal static IWindowService WindowService { get; private set; }

        App()
        {
            InitializeComponent();
            Exit += App_Exit;
            Startup += App_Startup;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Путь к локальному директорию приложения
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.CurrentDirectory);

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
        private void SetLogSettings()
        {
            if (DiplomApp.Properties.Settings.Default.EnableDebugInfo)
                LogManager.Configuration.LoggingRules[0].EnableLoggingForLevels(LogLevel.Trace, LogLevel.Debug);
            else
                LogManager.Configuration.LoggingRules[0].DisableLoggingForLevels(LogLevel.Trace, LogLevel.Debug);
        }
    }
}
