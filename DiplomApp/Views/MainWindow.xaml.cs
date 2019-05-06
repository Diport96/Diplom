using ClientApp;
using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.Server;
using DiplomApp.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(string username)
        {
            InitializeComponent();

            HelloLabel.Content = $"Здравствуйте {username}";
            StackOfDevices.ItemsSource = ControllersFactory.GetControllers();
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            new AuthentificationWindow().Show();
            Close();
        }
        private void DeviceItemSettings_Click(object sender, RoutedEventArgs e)
        {
            var device = ((e.Source as Button).DataContext as Controller);

            if (device is Switch)
                new SwitchSettingsWindow(device.ID).ShowDialog();
            else if (device is Sensor)
                new SensorSettingsWindow(device.ID).ShowDialog();
        }
        private void Application_Settings_Click(object sender, RoutedEventArgs e)
        {
            new ApplicationSettingsWindow().ShowDialog();
        }

        private async void Server_Start_Stop_Click(object sender, RoutedEventArgs e)
        {
            var server = ServerDevice.Instance;            
            if(server.IsRun)
            {
                (sender as Button).Content = "Запуск сервера";
                await server.StopAsync();
            }
            else
            {
                (sender as Button).Content = "Остановка сервера";
            }
        }
    }
}
