using ClientApp;
using DiplomApp.Controllers;
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

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            new AuthentificationWindow().Show();
            Close();
        }
    }

}
