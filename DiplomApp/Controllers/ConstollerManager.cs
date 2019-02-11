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
        public static void Remove(string id)
        {
            var control = controllers.Find(x => x.ID == id);
            if (control == null)
                logger.Error($"Ошибка удаления экземпляра микроконтроллера: не удалось найти экземпляр с идентификатором: {id}");
            else
                controllers.Remove(control);
        }
        public static IEnumerable<Controller> GetControllers()
        {
            return controllers.AsReadOnly();
        }
    }
}
