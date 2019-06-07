using DiplomApp.ViewModels;
using System.Windows;

namespace DiplomApp.Views
{
    /// <summary>
    /// Представляет диалоговое окно настроек датчика
    /// </summary>
    public partial class SensorSettingsWindow : Window
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="deviceId">Идентификатор датчика для которого выполняется настройка</param>
        public SensorSettingsWindow(string deviceId)
        {
            InitializeComponent();
            DataContext = new SensorSettingsViewModel(this, deviceId);
        }      
    }
}
