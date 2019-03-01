using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetOfConstants;
using Newtonsoft.Json;
using NLog;
using System.Reflection;
using DiplomApp.Controllers;

namespace DiplomApp.Server.Requsests
{
    [RequestType(MessageTypes.REQUSET_TO_CONNECT)]
    class ConnectHandler : IRequestHandler
    {
        private static readonly IEnumerable<Type> Types;
        private static ConnectHandler instance;
        public static ConnectHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConnectHandler();
                return instance;
            }
        }

        private ConnectHandler() { }
        static ConnectHandler()
        {
            Types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(Controller));
        }

        public void Execute(Dictionary<string, string> pairs)
        {
            pairs.TryGetValue("Type", out string t);
            pairs.TryGetValue("Topic", out string topic);
            pairs.TryGetValue("Class", out string classData);           
            var type = GetCType(t);
            var controller = JsonConvert.DeserializeObject(classData, type) as Controller;
            ControllerManager.Add(controller);
            var res = new Dictionary<string, string>
            {
                {"Message_Type", MessageTypes.PERMIT_TO_CONNECT },
                {"ID", controller.ID }
            };
            ServerDevice.Instance.SendMessage(res, topic);
        }

        private Type GetCType(string Type)
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
