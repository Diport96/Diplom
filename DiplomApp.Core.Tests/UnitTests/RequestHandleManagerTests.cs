using DiplomApp.Server.RequsestHandlers;
using DiplomApp.Server.SetOfConstants;
using NUnit.Framework;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    public class RequestHandleManagerTests
    {
        [Test]
        public void GetRequestHandler_TryGetConnectHandler_ConnectHandlerReturned()
        {
            Dictionary<string, string> request = new Dictionary<string, string>
            {
                { "Message_Type", MessageTypes.REQUSET_TO_CONNECT }
            };

            var handleManager = new RequestHandleManager();
            var handler = handleManager.GetRequestHandler(request);

            Assert.That(handler, Is.InstanceOf<ConnectHandler>());
        }
    }
}
