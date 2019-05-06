using DiplomApp.Controllers.Models;
using DiplomApp.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiplomApp.Controllers
{
    static class ControllersFactory
    {
        private static readonly RegisteredDeviceContext database;
        private static readonly ObservableCollection<Controller> controllers;
        private static readonly IEnumerable<Type> Types;
        private static readonly Logger logger;

        static ControllersFactory()
        {
            database = new RegisteredDeviceContext();
            controllers = new ObservableCollection<Controller>();
            Types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(Controller));
            logger = LogManager.GetCurrentClassLogger();
            App.Server.ServerStoped += Server_ServerStoped;
        }

        private static void Server_ServerStoped(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                controllers.Clear();
            });
        }

        public static void Create(Controller controller, string controllerType)
        {
            try
            {
                //!!! Найти способ оптимизации запроса            
                if (!database.RegisteredDevices.Any(x => x.ID == controller.ID))
                {
                    database.RegisteredDevices.Add(new RegisteredDeviceInfo(
                        controller.ID,
                        controller.Name,
                        controllerType,
                        DateTime.Now
                        ));

                    database.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.Fatal(e, e.Message);
                throw;
            }

            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                controllers.Add(controller);
            });
        }
        public static void Remove(string id)
        {
            var control = controllers.SingleOrDefault(x => x.ID == id);
            if (control == null)
                logger.Error($"Ошибка удаления экземпляра микроконтроллера: не удалось найти экземпляр с идентификатором: {id}");
            else
                Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    controllers.Remove(control);
                });
        }
        public static IEnumerable<Controller> GetControllers()
        {
            return controllers;
        }
        public static Controller GetById(string id)
        {
            var res = controllers.FirstOrDefault(x => x.ID == id);
            return res;
        }
        public static RegisteredDeviceInfo GetControllerInfo(string id)
        {
            var res = database.RegisteredDevices.FirstOrDefault(x => x.ID == id);
            return res;
        }
        public static Type GetType(string Type)
        {
            foreach (var t in Types)
            {
                if (t.Name == Type)
                    return t;
            }
            throw new InvalidControllerTypeException("Не удалось определить тип контроллера, возможно название класса не совпадает с названием типа контроллера");
        }
    }
}
