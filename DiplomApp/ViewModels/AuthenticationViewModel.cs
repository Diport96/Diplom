using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.ViewModels.Commands;
using DiplomApp.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiplomApp.ViewModels
{
    class AuthenticationViewModel : INotifyPropertyChanged
    {
        private AsyncRelayCommand signInCommand;
        private readonly Window ownerWindow;
        private string login;
        private string attemptMessage;
        private bool attemptShow;
        private bool isSignInButtonEnabled;
        private Visibility circualrBarIsVisible;
        public event PropertyChangedEventHandler PropertyChanged;

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
        public bool IsSignInButtonEnabled
        {
            get { return isSignInButtonEnabled; }
            set
            {
                isSignInButtonEnabled = value;
                OnPropertyChanged("IsSignInButtonEnabled");
            }
        }
        public Visibility CircualrBarIsVisible
        {
            get { return circualrBarIsVisible; }
            set
            {
                circualrBarIsVisible = value;
                OnPropertyChanged("CircualrBarIsVisible");
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

        public AuthenticationViewModel(Window owner)
        {
            ownerWindow = owner;
            IsSignInButtonEnabled = true;
            CircualrBarIsVisible = Visibility.Hidden;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private async Task SignIn(string login, string password)
        {
            BeginProcessing();   
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                AttemptMessage = "Поля логина и пароля не должны быть пустыми";
                AttemptShow = true;
                EndProcessing();
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
                    EndProcessing();
                }
            }
        }
        private void RedirectToMainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp = null)
        {
            new MainWindow(username, isLocalSession, connectToWebApp).Show();
            ownerWindow.Close();
        }
        private void BeginProcessing()
        {
            IsSignInButtonEnabled = false;
            CircualrBarIsVisible = Visibility.Visible;
        }
        private void EndProcessing()
        {
            IsSignInButtonEnabled = true;
            CircualrBarIsVisible = Visibility.Hidden;
        }
    }
}
