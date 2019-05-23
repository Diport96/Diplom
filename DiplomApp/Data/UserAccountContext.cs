using DiplomApp.Accounts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Data
{
    /// <summary>
    /// Представляет контекст данных для аккаунтов пользователей, для создания сессии с базой данных 
    /// </summary>
    public class UserAccountContext : DbContext
    {
        /// <summary>
        /// Сущность данных, в которой хранится информация 
        /// об аккаунтах пользователей
        /// </summary>
        public DbSet<UserAccount> UserAccounts { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public UserAccountContext() : base("SqlDatabaseConnection") { }
    }
}
