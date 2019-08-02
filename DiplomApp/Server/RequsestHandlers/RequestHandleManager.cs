using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DiplomApp.Server.RequsestHandlers
{
    class RequestHandleManager : IRequestHandleManager
    {
        private static readonly IEnumerable<Type> SupportedRequestHandlers;

        static RequestHandleManager()
        {
            SupportedRequestHandlers = GetRequestHandlersFromAssembly();
        }

        public IRequestHandler GetRequestHandler(Dictionary<string, string> keyValuePairs)
        {
            keyValuePairs.TryGetValue("Message_Type", out string msgType);
            var type = SupportedRequestHandlers.FirstOrDefault(x => (x.GetCustomAttribute(typeof(RequestTypeAttribute)) as RequestTypeAttribute).MessageType == msgType);
            if (type == null)
                throw new HandlerNotFindException($"Не удалось найти обработчик события соответствующий запросу: {msgType}");
            var prop = type.GetProperty("Instance");
            if (prop == null)
                throw new NotImplementedException($"В классе {type.Name} не реализован паттерн Singleton");
            var getClass = prop.GetMethod.Invoke(null, null) as IRequestHandler;
            return getClass;
        }

        private static IEnumerable<Type> GetRequestHandlersFromAssembly()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where
                (x => x.GetInterface(nameof(IRequestHandler)) != null &&
                x.GetCustomAttributes(typeof(RequestTypeAttribute), true).Length == 1);
        }
    }
}
