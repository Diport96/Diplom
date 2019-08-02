using DiplomApp.Server.RequsestHandlers;
using DiplomApp.Server.SetOfConstants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestClass]
    public class RequestHandleManagerTests
    {
        [TestMethod]
        public void GetRequestHandler_TryGetConnectHandler_ConnectHandlerReturned()
        {
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                {"Message_Type", MessageTypes.REQUSET_TO_CONNECT }
            };

            var handleManager = new RequestHandleManager();
            var handler = handleManager.GetRequestHandler(request);

            Assert.IsInstanceOfType(handler, typeof(ConnectHandler));
        }
    }
}
