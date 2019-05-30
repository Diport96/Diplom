using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.ViewModels.Commands;
using DiplomApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiplomApp.ViewModels
{
    class AuthenticationViewModel : INotifyPropertyChanged
    {
        private AsyncRelayCommand signInCommand;
        private readonly Window owner;
        private string login;
        private string attemptMessage;
        private bool attemptShow;

        public string AttemptMessage
        {
            get { return attemptMessage; }
            set
            {
                attemptMessage = value;
                OnPropertyChanged("AttemptMessage");
            }
        }
        public bool AttemptShow
        {
            get { return attemptShow; }
            set
            {
                attemptShow = value;
                OnPropertyChanged("AttemptShow");
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged("Login");
            }
        }
        public AsyncRelayCommand SignInCommand
        {
            get
            {
                return signInCommand ??
                    (signInCommand = new AsyncRelayCommand(obj => SignIn(Login, (obj as PasswordBox).Password)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AuthenticationViewModel(Window owner)
        {
            this.owner = owner;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async Task SignIn(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                AttemptMessage = "Поля логина и пароля не должны быть пустыми";
                AttemptShow = true;
            }

            //!!! Await exception handle
            if (await API.LoginAsync(login, password))
            {
                if (!AccountManager.CheckIfAccountExists(login))
                {
                    var acc = AccountManager.CreateAccount(login, password);
                }

                if (AccountManager.Login(login, password))
                    RedirectToMainWindow(login, false);
            }
            else
            {
                if (AccountManager.Login(login, password))
                {
                    async Task<bool> connectToWebApp() => await API.LoginAsync(login, password);
                    RedirectToMainWindow(login, true, connectToWebApp);
                }
                else
                {
                    AttemptMessage = "Неправильные логин или пароль";
                    AttemptShow = true;
                }
            }
        }
        private void RedirectToMainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp = null)
        {
            new MainWindow(username, isLocalSession, connectToWebApp).Show();
            owner.Close();
        }
    }
}
