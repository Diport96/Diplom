using DiplomApp.Server;
using Moq;
using NUnit.Framework;
using System.ComponentModel;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    public class MqttManagerTests
    {
        [Test]
        public async Task IsRunProperty_RunAndStopManager_TrueAfterRunAndFalseAfterStop()
        {
            IMqttComponent fakeServer = Mock.Of<IMqttComponent>();
            IMqttClientComponent fakeClient = Mock.Of<IMqttClientComponent>();
            MqttManager mqttManager = new MqttManager(fakeServer, fakeClient);

            await mqttManager.RunAsync();
            Assert.That(mqttManager.IsRun, Is.True);

            await mqttManager.StopAsync();
            Assert.That(mqttManager.IsRun, Is.False);
        }

        [Test]
        public async Task MqttProtocolStarted_SubscribeDelegateAndInvokeStartAsync_DelegateHasBeenInvokedOnce()
        {
            int invokedEvenCounter = 0;
            var fakeServer = Mock.Of<IMqttComponent>(x => x.RunAsync() == Task.FromResult(true));
            var fakeClient = Mock.Of<IMqttClientComponent>(x => x.RunAsync() == Task.FromResult(true));
            MqttManager mqttManager = new MqttManager(fakeServer, fakeClient);
            mqttManager.MqttProtocolStarted += delegate (object? sender, EventArgs e)
            {
                invokedEvenCounter++;
            };

            await mqttManager.RunAsync();

            Assert.That(invokedEvenCounter, Is.EqualTo(1));
        }

        [Test]
        public async Task MqttProtocolStoped_SubscribeDelegateAndInvokeStartAndStopAsync_DelegateHasBeenInvokedOnce()
        {
            int invokedEvenCounter = 0;
            var fakeServer = Mock.Of<IMqttComponent>();
            var fakeClient = Mock.Of<IMqttClientComponent>();
            MqttManager mqttManager = new MqttManager(fakeServer, fakeClient);
            mqttManager.MqttProtocolStoped += delegate (object? sender, EventArgs e)
            {
                invokedEvenCounter++;
            };

            await mqttManager.RunAsync();
            await mqttManager.StopAsync();

            Assert.That(invokedEvenCounter, Is.EqualTo(1));
        }

        [Test]
        public async Task PropertyChanged_SubscribeDelegateAndInvokeStartAsync_DelegateHasBeenInvokedOnce()
        {
            int invokedEvenCounter = 0;
            var fakeServer = Mock.Of<IMqttComponent>();
            var fakeClient = Mock.Of<IMqttClientComponent>();
            MqttManager mqttManager = new MqttManager(fakeServer, fakeClient);
            mqttManager.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                invokedEvenCounter++;
            };

            await mqttManager.RunAsync();

            Assert.That(invokedEvenCounter, Is.EqualTo(1));
        }
    }
}
