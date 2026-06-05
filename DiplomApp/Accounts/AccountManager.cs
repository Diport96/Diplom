using DiplomApp.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Security.Cryptography;

namespace DiplomApp.Accounts
{
    internal class AccountManager : IAccountManager
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly UserAccountContext database;
        private static AccountManager? _instance;

        public IAccount? CurrentUser { get; set; }
        public static AccountManager Instance => _instance ??= new AccountManager();

        public AccountManager()
        {
            database = new UserAccountContext();
            database.Database.EnsureCreated();
        }

        // Конструктор для тестов: принимает заранее сконфигурированный контекст
        internal AccountManager(UserAccountContext context)
        {
            database = context;
            database.Database.EnsureCreated();
        }

        public IAccount CreateAccount(string login, string password)
        {
            var result = database.UserAccounts.FirstOrDefault(x => x.Login == login);
            if (result != null)
                return result;

            using var deriveBytes = new Rfc2898DeriveBytes(password, 20, 1, HashAlgorithmName.SHA1);
            byte[] salt = deriveBytes.Salt;
            byte[] key = deriveBytes.GetBytes(20);

            var user = new UserAccount
            {
                Login = login,
                Key = key,
                Salt = salt
            };

            database.UserAccounts.Add(user);
            database.SaveChanges();

            return user;
        }

        public async Task<bool> CheckIfAccountExists(string login)
        {
            return await database.UserAccounts.AnyAsync(x => x.Login == login);
        }

        public bool Login(string login, string password)
        {
            UserAccount? result;
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
            return false;
        }

        public async Task<bool> LoginAsync(string login, string password)
        {
            UserAccount? result;
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
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        private UserAccount? AuthenticateUser(string login, string password)
        {
            UserAccount? userAccount = database.UserAccounts.FirstOrDefault(x => x.Login == login);
            if (userAccount != null)
            {
                using var deriveBytes = new Rfc2898DeriveBytes(password, userAccount.Salt!, 1, HashAlgorithmName.SHA1);
                byte[] newKey = deriveBytes.GetBytes(20);
                if (!newKey.SequenceEqual(userAccount.Key!))
                    return null;
            }
            return userAccount;
        }

        private async Task<UserAccount?> AuthenticateUserAsync(string login, string password)
        {
            UserAccount? userAccount = await database.UserAccounts.FirstOrDefaultAsync(x => x.Login == login);
            if (userAccount != null)
            {
                using var deriveBytes = new Rfc2898DeriveBytes(password, userAccount.Salt!, 1, HashAlgorithmName.SHA1);
                byte[] newKey = deriveBytes.GetBytes(20);
                if (!newKey.SequenceEqual(userAccount.Key!))
                    return null;
            }
            return userAccount;
        }
    }
}
