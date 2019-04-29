using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;
using SetOfConstants;
using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using NLog;

namespace DiplomApp.Server.RequsestHandlers
{
    [RequestType(SetOfConstants.MessageTypes.DISTRIBUTION_OF_VALUES)]
    class DistributeHandler : IRequestHandler
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
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString);
            database = client.GetDatabase("DevicesData");
        }

        public void Run(Dictionary<string, string> pairs)
        {
            var info = ControllersFactory.GetControllerInfo(pairs["ID"]);
            if (info == null)
            {
                logger.Error("В базе данных отсутствует информация о контроллере");
                return;
            }
            string deviceType = info.DeviceType;            
            switch (deviceType)
            {
                case "Sensor":
                    {
                        var controller = ControllersFactory.GetById(pairs["ID"]) as Sensor;
                        double.TryParse(pairs["Value"], out double value); //!!! Handle Exception
                        controller.Value = value;                        
                        break;
                    }
                case "Switch":
                    {
                        var controller = ControllersFactory.GetById(pairs["ID"]) as Switch;
                        bool.TryParse(pairs["Value"], out bool value); //!!! Handle Exception
                        controller.Value = value;                        
                        break;
                    }
                default:
                    {
                        logger.Error("Не удалось определить тип контроллера");
                        return;
                    }                    
            }

            //var deviceInfo = ControllersFactory.GetControllerInfo(pairs["ID"]);
            //if(deviceInfo == null)
            //{
            //    logger.Error("В базе данных отсутствует информация о контроллере");
            //}
            //ControllersFactory.GetType(deviceInfo.DeviceType);

           
            pairs.Remove("Topic");
            BsonDocument element = new BsonDocument(pairs);
            try
            {
                database.GetCollection<BsonDocument>(deviceType).InsertOneAsync(element).Wait(); //!!!
            }
            catch(Exception e)
            {
                logger.Fatal(e, e.Message);
            }
        }
    }
}
