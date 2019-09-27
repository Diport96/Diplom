using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.ViewModels.Services
{
    interface IWindowService
    {
        void OpenAuthenticationWindow();
        void CloseAuthenticationWindow();
        void OpenMainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp);
        void CloseMainWindow();
        bool? OpenAppSettingsDialogWindow();
        Controller OpenSelectSensorDialogWindow();
        bool? OpenSensorSettingsDialogWindow(string deviceId);
        bool? OpenSwitchSettingsDialogWindow(string deviceId);
    }
}
