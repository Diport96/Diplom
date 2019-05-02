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
    public partial class ApplicationSettingsWindow : Window
    {
        public ApplicationSettingsWindow()
        {
            InitializeComponent();

            WebAppAddressTextBox.Text = Properties.Settings.Default.WebAppDomain;
            AutoSendDataCheckBox.IsChecked = Properties.Settings.Default.AutoSendDataToWebApp;
            EnableDebugInfoCheckBox.IsChecked = Properties.Settings.Default.EnableDebugInfo;

            AutoSendDataCheckBox_CheckChanged(this, null);
            AutoSendDataCheckBox.Checked += AutoSendDataCheckBox_CheckChanged;
            AutoSendDataCheckBox.Unchecked += AutoSendDataCheckBox_CheckChanged;
        }

        private void AutoSendDataCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (AutoSendDataCheckBox.IsChecked.HasValue)
            {
                if (AutoSendDataCheckBox.IsChecked.Value)
                    AutoSendDataSettings.IsEnabled = true;
                else if (!AutoSendDataCheckBox.IsChecked.Value)
                    AutoSendDataSettings.IsEnabled = false;
            }
        }
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WebAppDomain = WebAppAddressTextBox.Text;
            Properties.Settings.Default.AutoSendDataToWebApp = AutoSendDataCheckBox.IsChecked.Value;
            Properties.Settings.Default.AutoSendDataEvery = CalculateTimeToAutoSendData();
            Properties.Settings.Default.EnableDebugInfo = EnableDebugInfoCheckBox.IsChecked.Value;

            Properties.Settings.Default.Save();
            DialogResult = true;
        }
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private TimeSpan CalculateTimeToAutoSendData()
        {
            switch (AutoSendDataComboBox.SelectedIndex)
            {
                case 0:
                    return new TimeSpan(0, 30, 0);
                case 1:
                    return new TimeSpan(1, 0, 0);
                case 2:
                    return new TimeSpan(2, 0, 0);
                case 3:
                    return new TimeSpan(5, 0, 0);
                case 4:
                    return new TimeSpan(16, 0, 0);
                case 5:
                    return new TimeSpan(1, 0, 0, 0);
                case 6:
                    return new TimeSpan(3, 0, 0, 0);
                case 7:
                    return new TimeSpan(7, 0, 0, 0);
                default:
                    return new TimeSpan(1, 0, 0, 0);
            }
        }
    }
}
