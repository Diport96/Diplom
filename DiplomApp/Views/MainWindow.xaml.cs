using ClientApp;
using DiplomApp.Accounts;
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
        private readonly ServerDevice server;
        public MainWindow(string username)
        {
            InitializeComponent();

            server = App.Server;
            HelloLabel.Content = $"Здравствуйте {username}";
            StackOfDevices.ItemsSource = ControllersFactory.GetControllers();
            SetServerStartStopButtonState(server, serverStartStopButton);
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            server.StopAsync().Wait();
        }
        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            AccountManager.Logout();
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
            var btn = (sender as Button);
            btn.IsEnabled = false;
            if (server.IsRun)
                await server.StopAsync();
            else
                await server.RunAsync();
            SetServerStartStopButtonState(server, btn);
            btn.IsEnabled = true;
        }

        private void SetServerStartStopButtonState(ServerDevice server, Button button)
        {
            if (server.IsRun)
                button.Content = "Остановить сервер";
            else
                button.Content = "Запустить сервер";
        }
    }
}
