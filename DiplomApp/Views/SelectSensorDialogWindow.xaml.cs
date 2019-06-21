using DiplomApp.ViewModels;
using System.Windows;

namespace DiplomApp.Views
{
    /// <summary>
    /// Представляет диалоговое окно выбора датчика для управления переключателем
    /// </summary>
    public partial class SelectSensorDialogWindow : Window
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public SelectSensorDialogWindow()
        {
            InitializeComponent();
            DataContext = new SelectSensorDialogViewModel(this);
        }
    }
}
