using DiplomApp;
using DiplomApp.Accounts;
using DiplomApp.Controllers;
using DiplomApp.Models;
using DiplomApp.Server.RequsestHandlers;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    [NonParallelizable] // использует глобальные App.ControllersFactory / App.UserAccountManager
    public class DistributeHandlerTests
    {
        private Mock<IControllersFactory> factory = null!;
        private Mock<IAccountManager> accountManager = null!;
        private Mock<IMongoCollection<BsonDocument>> collection = null!;
        private Mock<IMongoDatabase> database = null!;
        private DistributeHandler handler = null!;

        [SetUp]
        public void SetUp()
        {
            factory = new Mock<IControllersFactory>();
            accountManager = new Mock<IAccountManager>();
            accountManager.SetupGet(x => x.CurrentUser)
                .Returns(Mock.Of<IAccount>(a => a.Login == "user1"));

            collection = new Mock<IMongoCollection<BsonDocument>>();
            collection
                .Setup(c => c.InsertOneAsync(
                    It.IsAny<BsonDocument>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            database = new Mock<IMongoDatabase>();
            database
                .Setup(d => d.GetCollection<BsonDocument>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(collection.Object);

            App.ControllersFactory = factory.Object;
            App.UserAccountManager = accountManager.Object;

            handler = new DistributeHandler(database.Object);
        }

        private RegisteredDeviceInfo Info(string id, string deviceType)
            => new RegisteredDeviceInfo(id, "dev", deviceType, DateTime.Now);

        [Test]
        public void Run_UnknownInfo_DoesNotInsert()
        {
            // GetControllerInfo по умолчанию возвращает null
            var pairs = new Dictionary<string, string> { { "ID", "ghost" }, { "Value", "1" } };

            handler.Run(pairs);

            collection.Verify(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void Run_SensorValue_ParsesAndUpdatesControllerAndInserts()
        {
            var sensor = new Sensor("id-1", "S1", 0);
            factory.Setup(x => x.GetControllerInfo("id-1")).Returns(Info("id-1", "Sensor"));
            factory.Setup(x => x.GetType("Sensor")).Returns(typeof(Sensor));
            factory.Setup(x => x.GetById("id-1")).Returns(sensor);
            var pairs = new Dictionary<string, string>
            {
                { "ID", "id-1" }, { "Value", "42.5" }, { "Topic", "Devices/Sensors" }
            };

            handler.Run(pairs);

            Assert.That(sensor.Value, Is.EqualTo(42.5));
            collection.Verify(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public void Run_SwitchValue_ParsesBoolAndInserts()
        {
            var sw = new Switch("id-2", "Sw1", false);
            factory.Setup(x => x.GetControllerInfo("id-2")).Returns(Info("id-2", "Switch"));
            factory.Setup(x => x.GetType("Switch")).Returns(typeof(Switch));
            factory.Setup(x => x.GetById("id-2")).Returns(sw);
            var pairs = new Dictionary<string, string>
            {
                { "ID", "id-2" }, { "Value", "true" }, { "Topic", "Devices/Switches" }
            };

            handler.Run(pairs);

            Assert.That(sw.Value, Is.True);
            collection.Verify(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public void Run_NoCurrentUser_DoesNotCrashAndInserts()
        {
            accountManager.SetupGet(x => x.CurrentUser).Returns((IAccount)null!);
            var sensor = new Sensor("id-3", "S3", 0);
            factory.Setup(x => x.GetControllerInfo("id-3")).Returns(Info("id-3", "Sensor"));
            factory.Setup(x => x.GetType("Sensor")).Returns(typeof(Sensor));
            factory.Setup(x => x.GetById("id-3")).Returns(sensor);
            var pairs = new Dictionary<string, string>
            {
                { "ID", "id-3" }, { "Value", "10" }, { "Topic", "Devices/Sensors" }
            };

            Assert.DoesNotThrow(() => handler.Run(pairs));
            collection.Verify(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public void Run_NullController_DoesNotCrashAndInserts()
        {
            factory.Setup(x => x.GetControllerInfo("id-4")).Returns(Info("id-4", "Sensor"));
            factory.Setup(x => x.GetType("Sensor")).Returns(typeof(Sensor));
            factory.Setup(x => x.GetById("id-4")).Returns((Controller)null!);
            var pairs = new Dictionary<string, string>
            {
                { "ID", "id-4" }, { "Value", "5" }, { "Topic", "Devices/Sensors" }
            };

            Assert.DoesNotThrow(() => handler.Run(pairs));
            collection.Verify(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public void Run_InsertsIntoCollectionNamedByDeviceType()
        {
            var sensor = new Sensor("id-5", "S5", 0);
            factory.Setup(x => x.GetControllerInfo("id-5")).Returns(Info("id-5", "Sensor"));
            factory.Setup(x => x.GetType("Sensor")).Returns(typeof(Sensor));
            factory.Setup(x => x.GetById("id-5")).Returns(sensor);
            var pairs = new Dictionary<string, string>
            {
                { "ID", "id-5" }, { "Value", "1" }, { "Topic", "Devices/Sensors" }
            };

            handler.Run(pairs);

            database.Verify(d => d.GetCollection<BsonDocument>("Sensor", It.IsAny<MongoCollectionSettings>()), Times.Once);
        }
    }
}
