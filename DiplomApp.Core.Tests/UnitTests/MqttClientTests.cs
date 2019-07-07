using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MQTTnet.Client;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestClass]
    public class MqttClientTests
    {
        [TestMethod]
        public async Task IsRunProperty_RunAndStopClient_TrueAfterRunAndFalseAfterStop()
        {
            IMqttClient fakeClient = Mock.Of<IMqttClient>();
            IMqttClientOptions fakeClientOptions = Mock.Of<IMqttClientOptions>();
            Server.MqttClient mqttClient = new Server.MqttClient(fakeClient, fakeClientOptions);

            await mqttClient.RunAsync();
            Assert.IsTrue(mqttClient.IsRun);

            await mqttClient.StopAsync();
            Assert.IsFalse(mqttClient.IsRun);
        }
    }
}
