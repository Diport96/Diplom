using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.Data;
using DiplomApp.Server;
using DiplomApp.ViewModels;
using DiplomApp.Views;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            DataContext = new MainWindowViewModel(username, isLocalSession, connectToWebApp, this);
        }
    }
}
