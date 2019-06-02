using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiplomApp.Views
{
    /// <summary>
    /// Предсавляет диалоговое окно главного меню приложения
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="username">Имя пользователя, выполнившего аутентификацию</param>
        /// <param name="isLocalSession">Выполнена ли аутентификация локально</param>
        /// <param name="connectToWebApp">Функция переподключения к веб-серверу</param>
        public MainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(username, isLocalSession, connectToWebApp, this);
        }

        private void DeviceSettings_Button_Click(object sender, RoutedEventArgs e)
        {
            var device = ((e.Source as Button).DataContext as Controller);

            if (device is Switch)
                new SwitchSettingsWindow(device.ID).ShowDialog();
            else if (device is Sensor)
                new SensorSettingsWindow(device.ID).ShowDialog();
        }
    }
}
