using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Data
{
    class MongoDbInstance
    {
        private static MongoDbInstance instance;
        public static MongoDbInstance Instance
        {
            get
            {
                if (instance == null)
                    instance = new MongoDbInstance();
                return instance;
            }
        }
        public IMongoDatabase Database { get; private set; }

        private MongoDbInstance()
        {
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString);
            Database = client.GetDatabase("DevicesData");
        }

        public void SetDatabase(string connectionString)
        {
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase("DevicesData");
        }
    }
}
