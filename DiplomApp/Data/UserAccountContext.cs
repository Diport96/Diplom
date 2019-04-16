using DiplomApp.Accounts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Data
{

    class UserAccountContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }

        public UserAccountContext() : base("SqlDatabaseConnection") { }
    }
}
