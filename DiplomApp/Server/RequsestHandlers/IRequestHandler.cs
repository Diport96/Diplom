using System.Collections.Generic;

namespace DiplomApp.Server.RequsestHandlers
{
    interface IRequestHandler
    {
        void Run(Dictionary<string, string> pairs);
    }
}
