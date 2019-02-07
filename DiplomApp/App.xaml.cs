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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        App()
        {
            InitializeComponent();
            Exit += App_Exit;
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            logger.Info("Запуск приложения");            
        }
        private void App_Exit(object sender, ExitEventArgs e)
        {
            logger.Info("Завершение работы приложения");
        }
    }
}
