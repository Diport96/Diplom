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
        DbSet<Controller> Controllers { get; set; }

        public ControllerDbContext() : base("SqlDatabaseConnection") { }
    }
}
