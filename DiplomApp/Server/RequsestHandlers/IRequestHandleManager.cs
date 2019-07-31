using System.Collections.Generic;

namespace DiplomApp.Server.RequsestHandlers
{
    interface IRequestHandleManager
    {
        IRequestHandler GetRequestHandler(Dictionary<string, string> keyValuePairs);
    }
}
