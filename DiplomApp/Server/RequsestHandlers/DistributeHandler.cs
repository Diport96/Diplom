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

namespace DiplomApp.Server.RequsestHandlers
{
    [RequestType(SetOfConstants.MessageTypes.DISTRIBUTION_OF_VALUES)]
    class DistributeHandler : IRequestHandler
    {
        private readonly IMongoDatabase _database;
        private static DistributeHandler instance;
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
            _database = client.GetDatabase("DevicesData");
        }

        public void Run(Dictionary<string, string> pairs)
        {            
            switch (pairs["Topic"])
            {
                case Topics.SENSORS:
                    {
                        var controller = ControllersFactory.GetController(pairs["ID"]) as Sensor;
                        double.TryParse(pairs["Value"], out double value); //!!! Handle Exception
                        controller.Value = value;
                        break;
                    }
                case Topics.SWITCHES:
                    {
                        var controller = ControllersFactory.GetController(pairs["ID"]) as Switch;
                        bool.TryParse(pairs["Value"], out bool value); //!!! Handle Exception
                        controller.Value = value;
                        break;
                    }
                default:
                    break;
            }
            string topic = pairs["Topic"];
            pairs.Remove("Topic");
            BsonDocument element = new BsonDocument(pairs);
            _database.GetCollection<BsonDocument>(topic).InsertOneAsync(element).Wait(); //!!!
        }
    }
}
