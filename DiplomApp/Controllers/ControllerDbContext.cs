using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    class ControllerDbContext : DbContext
    {
        public DbSet<Controller> Controllers { get; set; }

        public ControllerDbContext() : base("SqlDatabaseConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Controller>().HasKey(pk => pk.ID);
            base.OnModelCreating(modelBuilder);
        }
    }
}
