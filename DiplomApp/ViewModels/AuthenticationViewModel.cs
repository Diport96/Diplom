﻿using ClientApp;
using DiplomApp.ViewModels.Commands;
using DiplomApp.ViewModels.Services;
using DiplomApp.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DiplomApp.App;

namespace DiplomApp.ViewModels
{
    class AuthenticationViewModel : BaseViewModel
    {
        private readonly IWindowService windowService;
        private AsyncRelayCommand signInCommand;
        private string login;
        private string attemptMessage;
        private bool attemptShow;
        private bool isSignInButtonEnabled;
        private Visibility circualrBarIsVisible;

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

        public AuthenticationViewModel(IWindowService windowService)
        {
            IsSignInButtonEnabled = true;
            CircualrBarIsVisible = Visibility.Hidden;
            this.windowService = windowService;
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
                if (!await UserAccountManager.CheckIfAccountExists(login))
                {
                    var acc = UserAccountManager.CreateAccount(login, password);
                }

                if (await UserAccountManager.LoginAsync(login, password))
                    RedirectToMainWindow(login, false);
            }
            else
            {
                if (await UserAccountManager.LoginAsync(login, password))
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
            windowService.OpenMainWindow(username, isLocalSession, connectToWebApp);
            windowService.CloseAuthenticationWindow();
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
