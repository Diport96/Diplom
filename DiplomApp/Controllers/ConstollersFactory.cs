using DiplomApp.Controllers.Models;
using DiplomApp.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    static class ControllersFactory
    {
        private static readonly RegisteredDeviceContext database;
        private static readonly List<Controller> controllers;        
        private static readonly Logger logger;

        static ControllersFactory()
        {
            database = new RegisteredDeviceContext();
            controllers = new List<Controller>();            
            logger = LogManager.GetCurrentClassLogger();
        }

        public static void Create(Controller controller, string controllerType)
        {
            //!!! Найти способ оптимизации запроса
            //!!! Exception handle
            if (!database.RegisteredDevices.Any(x=>x.ID == controller.ID))
            {
                database.RegisteredDevices.Add(new RegisteredDeviceInfo(
                    controller.ID,
                    controller.Name,
                    controllerType,
                    DateTime.Now
                    ));

                database.SaveChanges();                
            }
            controllers.Add(controller);            
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
