using DiplomApp.Controllers;
using DiplomApp.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly RegisteredDeviceInfo device;
        public SwitchSettingsWindow(string deviceId)
        {
            InitializeComponent();

            database = new RegisteredDeviceContext();
            var device = database.RegisteredDevices.FirstOrDefault(x => x.ID == deviceId);
            database.SwitchOptions.First(x => x.ID == device.ID);
            this.device = device;

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

                    DefaultValueRadioButton.Checked += ValueRadioButton_Checked;
                    SwitchDelayValueRadioButton.Checked += ValueRadioButton_Checked;
                    SensorIdValueRadioButton.Checked += ValueRadioButton_Checked;
                }
            }
        }

        private void ValueRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SetOptions();
        }
        private void SelectSensorId_Click(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new SelectSensorDialogWindow();
            if (dialogWindow.ShowDialog().Value)
            {
                if (dialogWindow.Answer != null)
                {
                    DeviceNameLabel.DataContext = dialogWindow.Answer;
                    DeviceNameLabel.Content = dialogWindow.Answer.Name;
                }
            }
        }
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            if (device != null)
            {
                device.Name = DeviceNameTextBox.Text;
                var options = database.SwitchOptions.First(x => x.ID == device.ID);

                if (DefaultValueRadioButton.IsChecked.Value)
                {
                    device.Options.Control = SwitchOptions.SwitchControl.No;
                    device.Options.DelayToSwitch = null;
                    device.Options.SensorId = null;
                    device.Options.ValueTo = null;
                }
                else if (SwitchDelayValueRadioButton.IsChecked.Value)
                {
                    TimeSpan timeSpan = new TimeSpan((int)HoursComboBox.SelectedItem, (int)MinutesComboBox.SelectedItem, (int)SecondsComboBox.SelectedItem);
                    bool? switchTo = null;
                    switch (SwitchToDelayComboBox.SelectedIndex)
                    {
                        case 0:
                            switchTo = true;
                            break;
                        case 1:
                            switchTo = false;
                            break;
                        default:
                            break;
                    }

                    device.Options.Control = SwitchOptions.SwitchControl.SwitchToDelay;
                    device.Options.DelayToSwitch = (int)timeSpan.TotalMilliseconds;
                    if (switchTo.HasValue)
                    {
                        device.Options.ValueTo = switchTo.Value;
                    }

                    device.Options.SensorId = null;
                }
                else if (SensorIdValueRadioButton.IsChecked.Value)
                {
                    if (DeviceNameLabel.DataContext != null)
                    {
                        bool? switchTo = null;
                        switch (SwitchToDelayComboBox.SelectedIndex)
                        {
                            case 0:
                                switchTo = true;
                                break;
                            case 1:
                                switchTo = false;
                                break;
                            default:
                                break;
                        }

                        device.Options.Control = SwitchOptions.SwitchControl.SwitchToSignal;
                        device.Options.SensorId = (DeviceNameLabel.DataContext as Controller).ID;
                        if (switchTo.HasValue)
                        {
                            device.Options.ValueTo = switchTo.Value;
                        }

                        device.Options.DelayToSwitch = null;
                    }
                }
            }

            database.SaveChanges();
            DialogResult = true;
        }
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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

    }
}
