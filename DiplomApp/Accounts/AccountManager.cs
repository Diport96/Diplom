using DiplomApp.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Accounts
{
    static class AccountManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly UserAccountContext database;
        public static UserAccount CurrentUser { get; set; }

        static AccountManager()
        {
            database = new UserAccountContext();
        }

        public static UserAccount CreateAccount(string login, string password)
        {
            var result = database.UserAccounts.FirstOrDefault(x => x.Login == login);
            if (result != null)
                return result;

            using (var deriveBytes = new Rfc2898DeriveBytes(password, 20))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] key = deriveBytes.GetBytes(20);

                var user = new UserAccount()
                {
                    Login = login,
                    Key = key,
                    Salt = salt
                };

                database.UserAccounts.Add(user);
                database.SaveChanges();

                return user;
            }
        }
        public static UserAccount AuthenticateUser(string login, string password)
        {
            // Exception of create database error code: -2146232060
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
            UserAccount result;
            try
            {
                result = AuthenticateUser(login, password);
            }
            catch (SqlException e)
            {
                logger.Fatal(e, e.Message);
                result = AuthenticateUser(login, password);
            }

            return result != null ? true : false;
        }
        public static void Logout()
        {
            CurrentUser = null;
        }
        public static UserAccount GetUser(string login)
        {
            return database.UserAccounts.FirstOrDefault(x => x.Login == login);
        }
    }
}
