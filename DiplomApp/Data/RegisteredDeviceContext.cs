using DiplomApp.Controllers;
using DiplomApp.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiplomApp.Data
{
    /// <summary>
    /// Представляет контекст данных для устройств, для создания сессии с базой данных 
    /// </summary>
    public class RegisteredDeviceContext : DbContext
    {
        /// <summary>
        /// Сущность данных, в которой хранится информация 
        /// об зарегистрированных в базе данных устройствах
        /// </summary>
        public DbSet<RegisteredDeviceInfo> RegisteredDevices { get; set; }

        /// <summary>
        /// Сущность данных, в которой хранится информация 
        /// об опциях зарегистрированных в базе данных устройств
        /// </summary>
        public DbSet<SwitchOptions> SwitchOptions { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public RegisteredDeviceContext() : base("SqlDatabaseConnection") { }

        /// <summary>
        /// Представляет Fluent API для Entity Framework
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegisteredDeviceInfo>().HasKey(pk => pk.ID);
            base.OnModelCreating(modelBuilder);
        }
    }
}
