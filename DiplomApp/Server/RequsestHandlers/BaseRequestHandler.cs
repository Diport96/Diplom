using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DiplomApp.Server.RequsestHandlers
{
    abstract class BaseRequestHandler
    {
        private static readonly IEnumerable<Type> SupportedRequestHandlers;

        static BaseRequestHandler()
        {
            SupportedRequestHandlers = Assembly.GetExecutingAssembly().GetTypes().Where
                (x => x.IsSubclassOf(typeof(BaseRequestHandler)) &&
                x.GetCustomAttributes(typeof(RequestTypeAttribute), true).Length == 1);
        }

        public abstract void Run(Dictionary<string, string> pairs);

        public static BaseRequestHandler GetRequestHandler(Dictionary<string, string> keyValuePairs)
        {
            keyValuePairs.TryGetValue("Message_Type", out string msgType);
            var type = SupportedRequestHandlers.FirstOrDefault(x => (x.GetCustomAttribute(typeof(RequestTypeAttribute)) as RequestTypeAttribute).MessageType == msgType);
            if (type == null)
                throw new HandlerNotFindException($"Не удалось найти обработчик события соответствующий запросу: {msgType}");
            var prop = type.GetProperty("Instance");
            if (prop == null)
                throw new NotImplementedException($"В классе {type.Name} не реализован паттерн Singleton");
            var getClass = prop.GetMethod.Invoke(null, null) as BaseRequestHandler;
            return getClass;
        }
    }
}
