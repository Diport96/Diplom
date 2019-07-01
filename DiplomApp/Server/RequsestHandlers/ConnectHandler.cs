using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiplomApp.Server.SetOfConstants;
using Newtonsoft.Json;
using NLog;
using System.Reflection;
using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;

namespace DiplomApp.Server.RequsestHandlers
{
    [RequestType(MessageTypes.REQUSET_TO_CONNECT)]
    class ConnectHandler : BaseRequestHandler
    {
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

        public override void Run(Dictionary<string, string> pairs)
        {
            pairs.TryGetValue("Type", out string t);
            pairs.TryGetValue("Topic", out string topic);
            pairs.TryGetValue("Class", out string classData);
            var type = ControllersFactory.GetType(t);
            var controller = JsonConvert.DeserializeObject(classData, type) as Controller;
            ControllersFactory.Create(controller, t);
            var res = ResponseManager.ConnackToDictionary(controller.ID);
            ServerDevice.Instance.SendMessage(res, topic).Wait();
        }
    }
}
