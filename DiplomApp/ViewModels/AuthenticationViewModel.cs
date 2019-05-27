using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.ViewModels.Commands;
using DiplomApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiplomApp.ViewModels
{
    class AuthenticationViewModel
    {
        private RelayCommand signInCommand;
        private readonly Window owner;

        public string AttemptMessage { get; set; }
        public bool AttemptShow { get; set; }
        public SignInData SignInData { get; set; }

        public RelayCommand SignInCommand
        {
            get
            {
                return signInCommand ??
                    (signInCommand = new RelayCommand(obj =>
                    {
                        SignInData signInData = obj as SignInData;
                        Task.Run(SignIn(signInData));
                    }));
            }
        }

        public AuthenticationViewModel(Window owner)
        {
            this.owner = owner;
        }

        private Func<Task> SignIn(SignInData signInData)
        {
            return async () =>
            {
                if (string.IsNullOrWhiteSpace(signInData.Login) || string.IsNullOrWhiteSpace(signInData.Password))
                {
                    AttemptMessage = "Поля логина и пароля не должны быть пустыми";
                    AttemptShow = true;
                }

                //!!! Await exception handle
                if (await API.LoginAsync(signInData.Login, signInData.Password))
                {
                    if (!AccountManager.CheckIfAccountExists(signInData.Login))
                    {
                        var acc = AccountManager.CreateAccount(signInData.Login, signInData.Password);
                    }

                    if (AccountManager.Login(signInData.Login, signInData.Password))
                        RedirectToMainWindow(signInData.Login, false);
                }
                else
                {
                    if (AccountManager.Login(SignInData.Login, signInData.Password))
                    {
                        async Task<bool> connectToWebApp() => await API.LoginAsync(signInData.Login, signInData.Password);
                        RedirectToMainWindow(signInData.Login, true, connectToWebApp);
                    }
                    else
                    {
                        AttemptMessage = "Неправильные логин или пароль";
                        AttemptShow = true;
                    }
                }
            };
        }
        private void RedirectToMainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp = null)
        {
            new MainWindow(username, isLocalSession, connectToWebApp).Show();
            owner.Close();
        }

    }
}
