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
    public partial class SwitchSettingsWindow : Window
    {
        private readonly RegisteredDeviceContext database;
        private string deviceId;

        public SwitchSettingsWindow(string deviceId)
        {
            InitializeComponent();

            database = new RegisteredDeviceContext();
            var device = database.RegisteredDevices.FirstOrDefault(x => x.ID == deviceId);
            this.deviceId = deviceId;

            DeviceNameTextBox.Text = device.Name;
            if (device != null)
            {
                var options = device.Options;

                if (options != null)
                {
                    switch (options.Control)
                    {
                        case SwitchOptions.SwitchControl.No:
                            DefaultValueRadioButton.IsChecked = true;
                            break;
                        case SwitchOptions.SwitchControl.SwitchToDelay:
                            SwitchDelayValueRadioButton.IsChecked = true;
                            break;
                        case SwitchOptions.SwitchControl.SwitchToSignal:
                            SensorIdValueRadioButton.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                    SetOptions();

                    DefaultValueRadioButton.Checked += DefaultValueRadioButton_Checked;
                    SwitchDelayValueRadioButton.Checked += DefaultValueRadioButton_Checked;
                    SensorIdValueRadioButton.Checked += DefaultValueRadioButton_Checked;
                }
            }
        }

        private void DefaultValueRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SetOptions();
        }

        private void SetOptions()
        {
            if (DefaultValueRadioButton.IsChecked.Value)
            {
                SwitchToDelayOptionsGrid.IsEnabled = false;
                SensorIdOptionsGrid.IsEnabled = false;
            }
            else if (SwitchDelayValueRadioButton.IsChecked.Value)
            {
                SwitchToDelayOptionsGrid.IsEnabled = true;
                SensorIdOptionsGrid.IsEnabled = false;
            }
            else if (SensorIdValueRadioButton.IsChecked.Value)
            {
                SwitchToDelayOptionsGrid.IsEnabled = false;
                SensorIdOptionsGrid.IsEnabled = true;
            }
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            var device = database.RegisteredDevices.FirstOrDefault(x => x.ID == deviceId);
            if (device != null)
            {
                device.Name = DeviceNameTextBox.Text;

                if (DefaultValueRadioButton.IsChecked.Value)
                {
                    device.Options.Control = SwitchOptions.SwitchControl.No;
                    device.Options.DelayToSwitch = null;
                    device.Options.SensorId = null;
                    device.Options.ValueTo = null;
                }
                else if (SwitchDelayValueRadioButton.IsChecked.Value)
                {
                    device.Options.Control = SwitchOptions.SwitchControl.SwitchToDelay;
                    
                }
                else if (SensorIdValueRadioButton.IsChecked.Value)
                {
                   
                }
            }
        }
    }
}
