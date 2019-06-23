using DiplomApp.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Accounts
{
    /// <summary>
    /// Предоставляет менеджер аккаунтов пользователей
    /// </summary>
    public static class AccountManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly UserAccountContext database;

        /// <summary>
        /// Хранит информацию об авторизованном пользователе
        /// </summary>
        public static UserAccount CurrentUser { get; set; }

        static AccountManager()
        {
            database = new UserAccountContext();
        }

        /// <summary>
        /// Создает новый аккакунт и возвращает его в качестве результата.
        /// Если такой аккаунт уже существует, возвращает его в качестве результата
        /// </summary>
        /// <param name="login">Логин от аккаунта</param>
        /// <param name="password">Пароль от аккаунта</param>
        /// <returns>Аккаунт пользователя</returns>
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

        /// <summary>
        /// Проверяет, существует ли аккаунт с указаным логином
        /// </summary>
        /// <param name="login">Логин от аккаунта</param>
        /// <returns>Существует ли аккаунт с указаным логином</returns>
        public static async Task<bool> CheckIfAccountExists(string login)
        {
            return await database.UserAccounts.AnyAsync(x => x.Login == login);
        }

        /// <summary>
        /// Выполняет аутентификацию пользователя
        /// </summary>
        /// <param name="login">Логин от аккаунта</param>
        /// <param name="password">Пароль от аккаунта</param>
        /// <returns>Удалоь ли выполнить аутентификацию</returns>
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

            if (result != null)
            {
                CurrentUser = result;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Выполняет аутентификацию пользователя асинхронно
        /// </summary>
        /// <param name="login">Логин от аккаунта</param>
        /// <param name="password">Пароль от аккаунта</param>
        /// <returns>Удалоь ли выполнить аутентификацию</returns>
        public static async Task<bool> LoginAsync(string login, string password)
        {
            UserAccount result;

            try
            {
                result = await AuthenticateUserAsync(login, password);
            }
            catch (SqlException e)
            {
                logger.Fatal(e, e.Message);
                result = await AuthenticateUserAsync(login, password);
            }

            if (result != null)
            {
                CurrentUser = result;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Выполняет выход из учетной записи пользователя
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }

        private static UserAccount AuthenticateUser(string login, string password)
        {
            // !!! Exception of create database error code: -2146232060
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
        private static async Task<UserAccount> AuthenticateUserAsync(string login, string password)
        {
            // !!! Exception of create database error code: -2146232060
            UserAccount userAccount = await database.UserAccounts.FirstOrDefaultAsync(x => x.Login == login);
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
    }
}
