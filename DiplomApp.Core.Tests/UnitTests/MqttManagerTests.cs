using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DiplomApp.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestClass]
    public class MqttManagerTests
    {
        [TestMethod]
        public async Task IsRunProperty_RunAndStopManager_TrueAfterRunAndFalseAfterStop()
        {
            IMqttComponent fakeServer = Mock.Of<IMqttComponent>();
            IMqttClientComponent fakeClient = Mock.Of<IMqttClientComponent>();
            MqttManager mqttManager = new MqttManager(fakeServer, fakeClient);

            await mqttManager.RunAsync();
            Assert.IsTrue(mqttManager.IsRun);

            await mqttManager.StopAsync();
            Assert.IsFalse(mqttManager.IsRun);
        }

        [TestMethod]
        public async Task MqttProtocolStarted_SubscribeDelegateAndInvokeStartAsync_DelegateHasBeenInvokedOnce()
        {
            int invokedEvenCounter = 0;
            var fakeServer = Mock.Of<IMqttComponent>(x => x.RunAsync() == Task.FromResult(true));
            var fakeClient = Mock.Of<IMqttClientComponent>(x => x.RunAsync() == Task.FromResult(true));
            MqttManager mqttManager = new MqttManager(fakeServer, fakeClient);
            mqttManager.MqttProtocolStarted += delegate (object sender, EventArgs e)
            {
                invokedEvenCounter++;
            };

            await mqttManager.RunAsync();

            Assert.AreEqual(1, invokedEvenCounter);
        }

        [TestMethod]
        public async Task MqttProtocolStoped_SubscribeDelegateAndInvokeStartAndStopAsync_DelegateHasBeenInvokedOnce()
        {
            int invokedEvenCounter = 0;
            var fakeServer = Mock.Of<IMqttComponent>();
            var fakeClient = Mock.Of<IMqttClientComponent>();
            MqttManager mqttManager = new MqttManager(fakeServer, fakeClient);
            mqttManager.MqttProtocolStoped += delegate (object sender, EventArgs e)
            {
                invokedEvenCounter++;
            };

            await mqttManager.RunAsync();
            await mqttManager.StopAsync();

            Assert.AreEqual(1, invokedEvenCounter);
        }

        [TestMethod]
        public async Task PropertyChanged_SubscribeDelegateAndInvokeStartAsync_DelegateHasBeenInvokedOnce()
        {
            int invokedEvenCounter = 0;
            var fakeServer = Mock.Of<IMqttComponent>();
            var fakeClient = Mock.Of<IMqttClientComponent>();
            MqttManager mqttManager = new MqttManager(fakeServer, fakeClient);
            mqttManager.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                invokedEvenCounter++;
            };

            await mqttManager.RunAsync();

            Assert.AreEqual(1, invokedEvenCounter);
        }
    }
}
