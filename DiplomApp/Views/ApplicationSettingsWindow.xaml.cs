using DiplomApp.ViewModels;
using System.Windows;

namespace DiplomApp.Views
{
    /// <summary>
    /// Представляет диалоговое окно настроек программы
    /// </summary>
    public partial class ApplicationSettingsWindow : Window
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public ApplicationSettingsWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationSettingsViewModel(this);
        }
    }
}
