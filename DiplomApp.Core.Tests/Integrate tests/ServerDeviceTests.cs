using System;
using System.Threading.Tasks;
using DiplomApp.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiplomApp.Core.Tests.Integrate_tests
{
    [TestClass]
    public class ServerDeviceTests
    {
        [TestMethod]
        public async Task Instance_RunAsyncIsRunCheck_TrueReturned()
        {
            var server = ServerDevice.Instance;
            await server.RunAsync();
            Assert.IsTrue(server.IsRun);
        }

        [TestMethod]
        public async Task Instance_StopAsyncIsRunCheck_FalseReturned()
        {
            var server = ServerDevice.Instance;
            await server.RunAsync();
            await server.StopAsync();
            Assert.IsFalse(server.IsRun);            
        }      
    }
}
