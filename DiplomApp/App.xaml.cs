using ClientApp;
using DiplomApp.Server;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DiplomApp
{
    public partial class App : Application
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static ServerDevice Server { get; private set; }

        App()
        {
            InitializeComponent();
            Exit += App_Exit;
            Startup += App_Startup;

            // Путь к локальному директорию приложения
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.CurrentDirectory);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Server = ServerDevice.Instance;                     
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal(e.ExceptionObject.ToString());
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            logger.Info("Запуск приложения");
        }
        private void App_Exit(object sender, ExitEventArgs e)
        {
            Server.StopAsync().Wait();
            logger.Info("Завершение работы приложения");
        }
        
    }
}
