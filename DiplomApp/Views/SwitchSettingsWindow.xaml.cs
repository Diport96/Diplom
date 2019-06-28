using DiplomApp.ViewModels;
using DiplomApp.ViewModels.Extensions;
using System.Windows;

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
            DataContext = new SwitchSettingsViewModel(deviceId, this.GetClosingWindowAction(), this.GetDialogResultWindowAction());
        }
    }
}
