using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.Data;
using DiplomApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            DataContext = new AuthenticationViewModel(this);
        }      
    }
}
