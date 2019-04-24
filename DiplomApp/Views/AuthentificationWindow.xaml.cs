using ClientApp;
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

            if (string.IsNullOrWhiteSpace(name))
            {
                // Handle
            }

            if (string.IsNullOrWhiteSpace(pass))
            {
                // Handle
            }

            if (await API.LoginAsync(name, pass))
            {
                if (!AccountManager.CheckIfAccountExists(name))
                    AccountManager.CreateAccount(name, pass);
                new MainWindow().Show();
                Close();
            }
            else
            {
                if(AccountManager.Login(name,pass))
                {
                    new MainWindow().Show();
                    Close();
                }
                // Handle
            }
        }
    }
}
