using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiplomApp.Server;
using DiplomApp.Server.SetOfConstants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

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
