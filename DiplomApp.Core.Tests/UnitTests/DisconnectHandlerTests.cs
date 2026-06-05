using DiplomApp;
using DiplomApp.Controllers;
using DiplomApp.Server.RequsestHandlers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    [NonParallelizable] // использует глобальный App.ControllersFactory
    public class DisconnectHandlerTests
    {
        [Test]
        public void Run_WithId_CallsRemoveWithThatId()
        {
            var factory = new Mock<IControllersFactory>();
            App.ControllersFactory = factory.Object;
            var pairs = new Dictionary<string, string> { { "ID", "dev-1" } };

            DisconnectHandler.Instance.Run(pairs);

            factory.Verify(x => x.Remove("dev-1"), Times.Once);
        }

        [Test]
        public void Run_WithoutId_CallsRemoveWithNull()
        {
            var factory = new Mock<IControllersFactory>();
            App.ControllersFactory = factory.Object;
            var pairs = new Dictionary<string, string>();

            DisconnectHandler.Instance.Run(pairs);

            factory.Verify(x => x.Remove(It.Is<string>(s => s == null)), Times.Once);
        }
    }
}
