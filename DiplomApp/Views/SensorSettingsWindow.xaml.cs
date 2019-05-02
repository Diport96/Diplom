using DiplomApp.Controllers;
using DiplomApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiplomApp.Views
{
    public partial class SensorSettingsWindow : Window
    {
        private readonly RegisteredDeviceContext database;
        private RegisteredDeviceInfo deviceInfo;

        public SensorSettingsWindow(string deviceId)
        {
            InitializeComponent();

            database = new RegisteredDeviceContext();
            deviceInfo = database.RegisteredDevices.First(x => x.ID == deviceId);
            DeviceNameTextBox.Text = deviceInfo.Name;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            deviceInfo.Name = DeviceNameTextBox.Text;
            database.SaveChanges();
            ControllersFactory.GetById(deviceInfo.ID).Name = DeviceNameTextBox.Text;

            DialogResult = true;
        }
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
