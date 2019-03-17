using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;

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

        public void Execute(Dictionary<string, string> pairs)
        {
            //!!! Доделать проверку на соответствие id с зарегестириованным устройством (SQL registered devices)

            pairs.Remove("Topic");
            BsonDocument element = new BsonDocument(pairs);
            _database.GetCollection<BsonDocument>("Termometers").InsertOneAsync(element).Wait();
        }
    }
}
