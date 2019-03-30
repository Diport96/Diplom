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
    class ControllerDbContext : DbContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static ControllerDbContext instance;
        public static ControllerDbContext Instance
        {
            get
            {
                if (instance == null) instance = new ControllerDbContext();
                return instance;
            }
        }
        private readonly CancellationTokenSource cancellationSource;

        public DbSet<Controller> Controllers { get; set; }

        private ControllerDbContext() : base("SqlDatabaseConnection")
        {
            cancellationSource = new CancellationTokenSource();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Controller>().HasKey(pk => pk.ID);
            base.OnModelCreating(modelBuilder);
        }

        //!!! синхронизация в отдельном потоке в цикле (не используется)
        private async Task Synchronize()
        {
            Action<CancellationToken> mainTask = (x) =>
            {
                logger.Debug("Запуск асинхронного потока для сервера");
                while (!x.IsCancellationRequested)
                {
                    SaveChangesAsync();
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            };
            await Task.Run(() => mainTask(cancellationSource.Token), cancellationSource.Token);
        }
    }
}
