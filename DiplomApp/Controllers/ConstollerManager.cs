using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    static class ControllerManager
    {
        private static readonly List<Controller> controllers;
        private static readonly Logger logger;

        static ControllerManager()
        {
            controllers = new List<Controller>();
            logger = LogManager.GetCurrentClassLogger();
        }
        
        public static void Add(Controller controller)
        {
            controllers.Add(controller);
            logger.Debug($"Добавлен новый контроллер. Общее количество: {controllers.Count}");
        }
        public static IEnumerable<Controller> GetControllers()
        {
            return controllers.AsReadOnly();
        }
    }
}
