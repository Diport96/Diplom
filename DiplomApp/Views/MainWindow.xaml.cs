using DiplomApp.ViewModels;
using DiplomApp.ViewModels.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DiplomApp.Views
{
    /// <summary>
    /// Предсавляет диалоговое окно главного меню приложения
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="username">Имя пользователя, выполнившего аутентификацию</param>
        /// <param name="isLocalSession">Выполнена ли аутентификация локально</param>
        /// <param name="connectToWebApp">Функция переподключения к веб-серверу</param>
        public MainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(username, isLocalSession, connectToWebApp, this.GetClosingWindowAction(), this.GetDialogResultWindowAction());
        }
    }
}
