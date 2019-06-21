using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.Data;
using DiplomApp.Server;
using DiplomApp.ViewModels;
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
    /// <summary>
    /// Представляет диалоговое окно настроек переключателя
    /// </summary>
    public partial class SwitchSettingsWindow : Window
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="deviceId">Идентификатор переключателя для которого выполняется настройка</param>
        public SwitchSettingsWindow(string deviceId)
        {
            InitializeComponent();
            DataContext = new SwitchSettingsViewModel(this, deviceId);
        }
    }
}
