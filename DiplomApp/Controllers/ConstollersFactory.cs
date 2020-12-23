using DiplomApp.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DiplomApp.Controllers
{
    internal class ControllersFactory : IControllersFactory
    {
        private static ControllersFactory _instance;
        private readonly RegisteredDeviceContext database;
        private readonly IEnumerable<Type> types;
        private readonly Logger logger;

        public static ControllersFactory Instance => _instance ?? (_instance = new ControllersFactory());
        public ObservableCollection<Controller> Controllers { get; }

        private ControllersFactory()
        {
            database = new RegisteredDeviceContext();
            Controllers = new ObservableCollection<Controller>();
            types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Controller)));
            logger = LogManager.GetCurrentClassLogger();
            App.Server.MqttProtocolStoped += Server_ServerStopped;
            database.Database.CreateIfNotExists();
        }

        public void Create(Controller controller, string controllerType)
        {
            try
            {
                RegisterDeviceInfoInDatabase(controller, controllerType, database);
            }
            catch (Exception e)
            {
                logger.Fatal(e, e.Message);
                throw;
            }

            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Controllers.Add(controller);
            });
        }
        public void Remove(string id)
        {
            var control = Controllers.SingleOrDefault(x => x.ID == id);
            if (control == null)
                logger.Error($"Ошибка удаления экземпляра микроконтроллера: не удалось найти экземпляр с идентификатором: {id}");
            else
                Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    Controllers.Remove(control);
                });
        }
        public IEnumerable<Controller> GetControllers()
        {
            return Controllers;
        } // TODO: Delete this code?
        public Controller GetById(string id)
        {
            var res = Controllers.FirstOrDefault(x => x.ID == id);
            return res;
        }
        public RegisteredDeviceInfo GetControllerInfo(string id)
        {
            var res = database.RegisteredDevices.FirstOrDefault(x => x.ID == id);
            return res;
        }
        public Type GetType(string type)
        {
            foreach (var t in types)
            {
                if (t.Name == type)
                    return t;
            }
            throw new InvalidControllerTypeException("Не удалось определить тип контроллера, возможно название класса не совпадает с названием типа контроллера");
        }

        private void RegisterDeviceInfoInDatabase(Controller controller, string controllerType, RegisteredDeviceContext context)
        {
            //TODO: Найти способ оптимизации запроса  
            if (!context.RegisteredDevices.Any(x => x.ID == controller.ID))
            {
                context.RegisteredDevices.Add(new RegisteredDeviceInfo(
                    controller.ID,
                    controller.Name,
                    controllerType,
                    DateTime.Now
                    ));

                context.SaveChanges();
            }
        }
        private void Server_ServerStopped(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Controllers.Clear();
            });
        }
    }
}
