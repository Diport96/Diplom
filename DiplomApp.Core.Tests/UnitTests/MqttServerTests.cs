using DiplomApp.Server;
using Moq;
using NUnit.Framework;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    public class MqttServerTests
    {
        [Test]
        public async Task IsRunProperty_RunAndStopServer_TrueAfterRunAndFalseAfterStop()
        {
            IMqttServerAdapter fakeServerAdapter = Mock.Of<IMqttServerAdapter>();
            Server.MqttServer mqttServer = new Server.MqttServer(fakeServerAdapter);

            await mqttServer.RunAsync();
            Assert.That(mqttServer.IsRun, Is.True);

            await mqttServer.StopAsync();
            Assert.That(mqttServer.IsRun, Is.False);
        }
    }
}
