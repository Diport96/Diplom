using DiplomApp.Controllers.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    class RegisteredDeviceContext : DbContext
    {
        public DbSet<RegisteredDeviceInfo> RegisteredDevices { get; set; }

        public RegisteredDeviceContext() : base("SqlDatabaseConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegisteredDeviceInfo>().HasKey(pk => pk.ID);
            base.OnModelCreating(modelBuilder);
        }
    }
}
