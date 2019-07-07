using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestClass]
    public class MqttServerTests
    {
        [TestMethod]
        public async Task IsRunProperty_RunAndStopServer_TrueAfterRunAndFalseAfterStop()
        {
            IMqttServer fakeServer = Mock.Of<IMqttServer>();
            IMqttServerOptions fakeServerOptions = Mock.Of<IMqttServerOptions>();
            Server.MqttServer mqttServer = new Server.MqttServer(fakeServer, fakeServerOptions);

            await mqttServer.RunAsync();
            Assert.IsTrue(mqttServer.IsRun);

            await mqttServer.StopAsync();
            Assert.IsFalse(mqttServer.IsRun);
        }
    }
}
