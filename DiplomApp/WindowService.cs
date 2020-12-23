using DiplomApp.Controllers;
using DiplomApp.ViewModels;
using DiplomApp.ViewModels.Services;
using DiplomApp.Views;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DiplomApp
{
    internal class WindowService : IWindowService
    {
        private AuthentificationWindow authenticationWindow;
        private MainWindow mainWindow;

        public void OpenAuthenticationWindow()
        {
            if (authenticationWindow == null)
            {
                authenticationWindow = new AuthentificationWindow
                {
                    DataContext = new AuthenticationViewModel(this)
                };
                authenticationWindow.Closed += (sender, e) => authenticationWindow = null;
                authenticationWindow.Show();
            }
        }
        public void CloseAuthenticationWindow()
        {
            if (authenticationWindow != null)
                authenticationWindow.Close();
        }
        public void OpenMainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp)
        {
            if (mainWindow == null)
            {
                mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(username, isLocalSession, connectToWebApp, this)
                };
                mainWindow.Closed += (sender, e) => mainWindow = null;
                mainWindow.Show();
            }
        }
        public void CloseMainWindow()
        {
            if (mainWindow != null)
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

        private static Action<bool> GetActionDialogResult(Window window)
        {
            return (result) =>
            {
                window.DialogResult = result;
            };
        }
    }
}
