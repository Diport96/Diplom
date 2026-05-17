using DiplomApp.Accounts;
using Microsoft.EntityFrameworkCore;

namespace DiplomApp.Data
{
    public class UserAccountContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(Properties.Settings.Default.SqlConnectionString);
    }
}
