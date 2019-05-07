﻿using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class AuthentificationWindow : Window
    {
        public AuthentificationWindow()
        {
            InitializeComponent();
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UsernameTextBox.Text, pass = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pass))
            {
                AttemptBox.Text = "Поля логина и пароля не должны быть пустыми";
            }

            //!!! Await exception handle
            if (await API.LoginAsync(name, pass))
            {
                if (!AccountManager.CheckIfAccountExists(name))
                {
                    var acc = AccountManager.CreateAccount(name, pass);
                    AccountManager.CurrentUser = acc;
                }
                RedirectToMainWindow(name);
            }
            else
            {
                if (AccountManager.Login(name, pass))
                {
                    AccountManager.CurrentUser = AccountManager.GetUser(name);
                    RedirectToMainWindow(name);
                }
                else
                {
                    AttemptBox.Text = "Неправильные логин или пароль";
                    Attempt.IsOpen = true;
                }
            }
        }

        private void RedirectToMainWindow(string username)
        {
            new MainWindow(username).Show();
            Close();
        }
    }
}
