using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Accounts
{
    interface IAccountManager
    {
        IAccount CurrentUser { get; set; }

        IAccount CreateAccount(string login, string password);

        Task<bool> CheckIfAccountExists(string login);

        bool Login(string login, string password);

        Task<bool> LoginAsync(string login, string password);

        void Logout();
    }
}
