using Moq;
using MQTTnet.Client;
using NUnit.Framework;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    public class MqttClientTests
    {
        [Test]
        public async Task IsRunProperty_RunAndStopClient_TrueAfterRunAndFalseAfterStop()
        {
            IMqttClient fakeClient = Mock.Of<IMqttClient>();
            MqttClientOptions fakeClientOptions = new MqttClientOptionsBuilder().Build();
            Server.MqttClient mqttClient = new Server.MqttClient(fakeClient, fakeClientOptions, Callback);

            await mqttClient.RunAsync();
            Assert.That(mqttClient.IsRun, Is.True);

            await mqttClient.StopAsync();
            Assert.That(mqttClient.IsRun, Is.False);
        }

        private Task Callback(MqttApplicationMessageReceivedEventArgs e) => Task.CompletedTask;
    }
}
