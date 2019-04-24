using DiplomApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Accounts
{
    static class AccountManager
    {
        private static readonly UserAccountContext database;

        static AccountManager()
        {
            database = new UserAccountContext();
        }

        public static CreateAccountState CreateAccount(string login, string password)
        {
            if (database.UserAccounts.Any(x => x.Login == login))
                return CreateAccountState.UserAlreadyExists;

            using (var deriveBytes = new Rfc2898DeriveBytes(password, 20))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] key = deriveBytes.GetBytes(20);

                database.UserAccounts.Add(new UserAccount()
                {
                    Login = login,
                    Key = key,
                    Salt = salt
                });
                database.SaveChanges();
            }

            return CreateAccountState.OK;
        }
        public static UserAccount GetUserAccount(string login, string password)
        {
            UserAccount userAccount = database.UserAccounts.FirstOrDefault(x => x.Login == login);
            if (userAccount != null)
            {
                using (var deriveBytes = new Rfc2898DeriveBytes(password, userAccount.Salt))
                {
                    byte[] newKey = deriveBytes.GetBytes(20);

                    if (!newKey.SequenceEqual(userAccount.Key))
                        return null;
                }

                return userAccount;
            }

            return null;
        }
        public static bool CheckIfAccountExists(string login)
        {
            return database.UserAccounts.Any(x => x.Login == login);
        }
        public static bool Login(string login, string password)
        {
            if (GetUserAccount(login, password) != null)
                return true;
            else
                return false;                            
        }
    }

    enum CreateAccountState
    {
        OK,
        UserAlreadyExists       
    }
}
