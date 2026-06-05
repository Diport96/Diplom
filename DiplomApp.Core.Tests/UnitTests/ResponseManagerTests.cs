using DiplomApp;
using DiplomApp.Controllers;
using DiplomApp.Server;
using DiplomApp.Server.SetOfConstants;
using Moq;
using NUnit.Framework;
using System;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    [NonParallelizable] // использует глобальный App.ControllersFactory
    public class ResponseManagerTests
    {
        private Mock<IControllersFactory> factory = null!;

        [SetUp]
        public void SetUp()
        {
            factory = new Mock<IControllersFactory>();
            App.ControllersFactory = factory.Object;
        }

        private void SetupInfo(string id, SwitchOptions options)
        {
            var info = new RegisteredDeviceInfo(id, "device", "Switch", DateTime.Now)
            {
                Options = options
            };
            factory.Setup(x => x.GetControllerInfo(id)).Returns(info);
        }

        [Test]
        public void ConnackToDictionary_ControlNo_IncludesControlNoWithoutExtraKeys()
        {
            SetupInfo("id-1", new SwitchOptions());

            var res = ResponseManager.ConnackToDictionary("id-1");

            Assert.That(res["Message_Type"], Is.EqualTo(MessageTypes.PERMIT_TO_CONNECT));
            Assert.That(res["ID"], Is.EqualTo("id-1"));
            Assert.That(res["Control"], Is.EqualTo(SwitchOptions.SwitchControl.No.ToString()));
            Assert.That(res.ContainsKey("Delay"), Is.False);
            Assert.That(res.ContainsKey("SensorId"), Is.False);
        }

        [Test]
        public void ConnackToDictionary_SwitchToDelay_IncludesDelayAndValueTo()
        {
            SetupInfo("id-2", new SwitchOptions(5000, true));

            var res = ResponseManager.ConnackToDictionary("id-2");

            Assert.That(res["Control"], Is.EqualTo(SwitchOptions.SwitchControl.SwitchToDelay.ToString()));
            Assert.That(res["Delay"], Is.EqualTo("5000"));
            Assert.That(res["ValueTo"], Is.EqualTo(true.ToString()));
        }

        [Test]
        public void ConnackToDictionary_SwitchToSignal_IncludesSensorIdAndValueTo()
        {
            SetupInfo("id-3", new SwitchOptions("sensor-7", false));

            var res = ResponseManager.ConnackToDictionary("id-3");

            Assert.That(res["Control"], Is.EqualTo(SwitchOptions.SwitchControl.SwitchToSignal.ToString()));
            Assert.That(res["SensorId"], Is.EqualTo("sensor-7"));
            Assert.That(res["ValueTo"], Is.EqualTo(false.ToString()));
        }

        [Test]
        public void ConnackToDictionary_UnknownDevice_ReturnsBaseDictWithoutCrash()
        {
            // GetControllerInfo по умолчанию возвращает null (мок без Setup)
            var res = ResponseManager.ConnackToDictionary("ghost");

            Assert.That(res["Message_Type"], Is.EqualTo(MessageTypes.PERMIT_TO_CONNECT));
            Assert.That(res["ID"], Is.EqualTo("ghost"));
            Assert.That(res.ContainsKey("Control"), Is.False);
        }

        [Test]
        public void SetSwitchStateToDictionary_ReturnsExpectedDictionary()
        {
            var res = ResponseManager.SetSwitchStateToDictionary("id-4", true);

            Assert.That(res["Message_Type"], Is.EqualTo(MessageTypes.CHANGE_SWITCH_STATE));
            Assert.That(res["ID"], Is.EqualTo("id-4"));
            Assert.That(res["Value"], Is.EqualTo(true.ToString()));
        }

        [Test]
        public void SetSwitchOptionsToDictionary_DelayMode_IncludesDelayAndValueTo()
        {
            var res = ResponseManager.SetSwitchOptionsToDictionary("id-5", new SwitchOptions(1000, false));

            Assert.That(res["Message_Type"], Is.EqualTo(MessageTypes.SET_NEW_SWITCH_OPTIONS));
            Assert.That(res["Control"], Is.EqualTo(SwitchOptions.SwitchControl.SwitchToDelay.ToString()));
            Assert.That(res["Delay"], Is.EqualTo("1000"));
            Assert.That(res["ValueTo"], Is.EqualTo(false.ToString()));
        }

        [Test]
        public void SetSwitchOptionsToDictionary_SignalMode_IncludesSensorIdAndValueTo()
        {
            var res = ResponseManager.SetSwitchOptionsToDictionary("id-6", new SwitchOptions("sensor-9", true));

            Assert.That(res["Control"], Is.EqualTo(SwitchOptions.SwitchControl.SwitchToSignal.ToString()));
            Assert.That(res["SensorId"], Is.EqualTo("sensor-9"));
            Assert.That(res["ValueTo"], Is.EqualTo(true.ToString()));
        }

        [Test]
        public void SetSwitchOptionsToDictionary_NoMode_IncludesControlOnly()
        {
            var res = ResponseManager.SetSwitchOptionsToDictionary("id-7", new SwitchOptions());

            Assert.That(res["Control"], Is.EqualTo(SwitchOptions.SwitchControl.No.ToString()));
            Assert.That(res.ContainsKey("Delay"), Is.False);
            Assert.That(res.ContainsKey("SensorId"), Is.False);
        }
    }
}
