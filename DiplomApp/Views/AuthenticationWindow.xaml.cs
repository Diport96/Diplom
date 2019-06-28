using DiplomApp.ViewModels;
using DiplomApp.ViewModels.Extensions;
using System.Windows;

namespace DiplomApp.Views
{
    /// <summary>
    /// Представляет диалоговое окно аутентификации пользователя
    /// </summary>
    public partial class AuthentificationWindow : Window
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public AuthentificationWindow()
        {
            InitializeComponent();
            DataContext = new AuthenticationViewModel(this.GetClosingWindowAction(), this.GetDialogResultWindowAction());
        }
    }
}
