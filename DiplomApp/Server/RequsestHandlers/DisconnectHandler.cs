using DiplomApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server.RequsestHandlers
{
    [RequestType(SetOfConstants.MessageTypes.REQUSET_TO_DISCONNECT)]
    class DisconnectHandler : IRequestHandler
    {
        private static DisconnectHandler instance;
        public static DisconnectHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new DisconnectHandler();
                return instance;
            }
        }

        private DisconnectHandler() { }

        public void Run(Dictionary<string, string> pairs)
        {
            pairs.TryGetValue("ID", out string id);
            ControllersFactory.Remove(id);
        }
    }
}
