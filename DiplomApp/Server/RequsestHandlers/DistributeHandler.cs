﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;
using DiplomApp.Server.SetOfConstants;
using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using NLog;
using DiplomApp.Accounts;
using DiplomApp.Data;

namespace DiplomApp.Server.RequsestHandlers
{
    [RequestType(SetOfConstants.MessageTypes.DISTRIBUTION_OF_VALUES)]
    class DistributeHandler : BaseRequestHandler
    {
        private readonly IMongoDatabase database;
        private static DistributeHandler instance;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static DistributeHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new DistributeHandler();
                return instance;
            }
        }

        private DistributeHandler()
        {
            database = MongoDbInstance.Instance.Database;
        }

        public override void Run(Dictionary<string, string> pairs)
        {
            var info = ControllersFactory.GetControllerInfo(pairs["ID"]);
            if (info == null)
            {
                logger.Error("В базе данных отсутствует информация о контроллере");
                return;
            }
            var deviceType = info.DeviceType;
            var type = ControllersFactory.GetType(deviceType);

            if (type == typeof(Sensor) || type.IsSubclassOf(typeof(Sensor)))
            {
                var controller = ControllersFactory.GetById(pairs["ID"]) as Sensor;
                double.TryParse(pairs["Value"], out double value);
                controller.Value = value;
            }
            else if (type == typeof(Switch) || type.IsSubclassOf(typeof(Switch)))
            {
                var controller = ControllersFactory.GetById(pairs["ID"]) as Switch;
                bool.TryParse(pairs["Value"], out bool value);
                controller.Value = value;
            }

            pairs.Remove("Topic");
            pairs.Add("User", App.UserAccountManager.CurrentUser.Login);
            pairs.Add("Name", info.Name);
            BsonDocument element = new BsonDocument(pairs);
            try
            {
                database.GetCollection<BsonDocument>(deviceType).InsertOneAsync(element).Wait(); //!!!
            }
            catch (Exception e)
            {
                logger.Fatal(e, e.Message);
                throw;
            }
        }
    }
}
