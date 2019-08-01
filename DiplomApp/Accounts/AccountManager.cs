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
    class AccountManager : IAccountManager
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly UserAccountContext database;
        private static AccountManager instance;

        /// <summary>
        /// Хранит информацию об авторизованном пользователе
        /// </summary>
        public IAccount CurrentUser { get; set; }
        public static AccountManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AccountManager();
                return instance;
            }
        }

        public AccountManager()
        {
            database = new UserAccountContext();
            database.Database.CreateIfNotExists();
        }

        /// <summary>
        /// Создает новый аккакунт и возвращает его в качестве результата.
        /// Если такой аккаунт уже существует, возвращает его в качестве результата
        /// </summary>
        /// <param name="login">Логин от аккаунта</param>
        /// <param name="password">Пароль от аккаунта</param>
        /// <returns>Аккаунт пользователя</returns>
        public IAccount CreateAccount(string login, string password)
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
        public async Task<bool> CheckIfAccountExists(string login)
        {
            return await database.UserAccounts.AnyAsync(x => x.Login == login);
        }

        /// <summary>
        /// Выполняет аутентификацию пользователя
        /// </summary>
        /// <param name="login">Логин от аккаунта</param>
        /// <param name="password">Пароль от аккаунта</param>
        /// <returns>Удалоь ли выполнить аутентификацию</returns>
        public bool Login(string login, string password)
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
        public async Task<bool> LoginAsync(string login, string password)
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
        public void Logout()
        {
            CurrentUser = null;
        }

        private UserAccount AuthenticateUser(string login, string password)
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
        private async Task<UserAccount> AuthenticateUserAsync(string login, string password)
        {
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
