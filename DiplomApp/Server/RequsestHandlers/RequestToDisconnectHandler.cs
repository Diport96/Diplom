using DiplomApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server.RequsestHandlers
{
    [RequestType(SetOfConstants.MessageTypes.REQUSET_TO_DISCONNECT)]
    class RequestToDisconnectHandler : IRequestHandler
    {
        private static RequestToDisconnectHandler instance;
        public static RequestToDisconnectHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new RequestToDisconnectHandler();
                return instance;
            }
        }

        private RequestToDisconnectHandler() { }

        public void Execute(Dictionary<string, string> pairs)
        {
            pairs.TryGetValue("ID", out string id);
            ControllerManager.Remove(id);
        }
    }
}
