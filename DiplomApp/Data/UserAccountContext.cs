using DiplomApp.Accounts;
using Microsoft.EntityFrameworkCore;

namespace DiplomApp.Data
{
    public class UserAccountContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }

        public UserAccountContext() { }

        // Конструктор для тестов: позволяет подставить InMemory-провайдер
        public UserAccountContext(DbContextOptions<UserAccountContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(Properties.Settings.Default.SqlConnectionString);
        }
    }
}
