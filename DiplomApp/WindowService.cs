using DiplomApp.Controllers;
using DiplomApp.ViewModels;
using DiplomApp.ViewModels.Services;
using DiplomApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiplomApp
{
    class WindowService : IWindowService
    {
        private AuthentificationWindow authentificationWindow;
        private MainWindow mainWindow;

        public void OpenAuthenticationWindow()
        {
            if (authentificationWindow == null)
            {
                authentificationWindow = new AuthentificationWindow
                {
                    DataContext = new AuthenticationViewModel(this)
                };
                authentificationWindow.Closed += Window_Closed;
                authentificationWindow.Show();
            }
        }
        public void CloseAuthenticationWindow()
        {
            authentificationWindow.Close();
        }
        public void OpenMainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp)
        {
            if (mainWindow == null)
            {
                mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(username, isLocalSession, connectToWebApp, this)
                };
                mainWindow.Closed += Window_Closed;
                mainWindow.Show();
            }
        }
        public void CloseMainWindow()
        {
            mainWindow.Close();
        }

        public bool? OpenAppSettingsDialogWindow()
        {
            var window = new ApplicationSettingsWindow();
            window.DataContext = new ApplicationSettingsViewModel(GetActionDialogResult(window));
            return window.ShowDialog();
        }
        public Controller OpenSelectSensorDialogWindow()
        {
            var window = new SelectSensorDialogWindow();
            var vm = new SelectSensorDialogViewModel(GetActionDialogResult(window));
            window.DataContext = vm;
            var dialogResult = window.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
                return vm.SelectedSensor;
            else
                return null;
        }
        public bool? OpenSensorSettingsDialogWindow(string deviceId)
        {
            var window = new SensorSettingsWindow();
            window.DataContext = new SensorSettingsViewModel(deviceId, GetActionDialogResult(window));
            return window.ShowDialog();
        }
        public bool? OpenSwitchSettingsDialogWindow(string deviceId)
        {
            var window = new SwitchSettingsWindow();
            window.DataContext = new SwitchSettingsViewModel(deviceId, GetActionDialogResult(window), this);
            return window.ShowDialog();
        }

        private Action<bool> GetActionDialogResult(Window window)
        {
            return (result) =>
            {
                window.DialogResult = result;
            };
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (sender is AuthentificationWindow) authentificationWindow = null;
            else if (sender is MainWindow) mainWindow = null;
        }
    }
}
