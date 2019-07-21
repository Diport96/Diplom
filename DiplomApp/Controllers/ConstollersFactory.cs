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
    class ControllersFactory : IControllersFactory
    {
        private static ControllersFactory instance;
        private readonly RegisteredDeviceContext database;
        private readonly IEnumerable<Type> Types;
        private readonly Logger logger;

        public static ControllersFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new ControllersFactory();
                return instance;
            }
        }
        public ObservableCollection<Controller> Controllers { get; }

        private ControllersFactory()
        {
            database = new RegisteredDeviceContext();
            Controllers = new ObservableCollection<Controller>();
            Types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Controller)));
            logger = LogManager.GetCurrentClassLogger();
            App.Server.MqttProtocolStoped += Server_ServerStoped;
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
        } //!!! Delete this code?
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
        public Type GetType(string Type)
        {
            foreach (var t in Types)
            {
                if (t.Name == Type)
                    return t;
            }
            throw new InvalidControllerTypeException("Не удалось определить тип контроллера, возможно название класса не совпадает с названием типа контроллера");
        }

        private void RegisterDeviceInfoInDatabase(Controller controller, string controllerType, RegisteredDeviceContext database)
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
        private void Server_ServerStoped(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Controllers.Clear();
            });
        }
    }
}
