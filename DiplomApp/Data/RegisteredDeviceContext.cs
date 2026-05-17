using DiplomApp.Controllers;
using Microsoft.EntityFrameworkCore;

namespace DiplomApp.Data
{
    public class RegisteredDeviceContext : DbContext
    {
        public DbSet<RegisteredDeviceInfo> RegisteredDevices { get; set; }
        public DbSet<SwitchOptions> SwitchOptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(Properties.Settings.Default.SqlConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegisteredDeviceInfo>().HasKey(pk => pk.ID);
            base.OnModelCreating(modelBuilder);
        }
    }
}
