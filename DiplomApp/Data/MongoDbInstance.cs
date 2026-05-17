using MongoDB.Driver;

namespace DiplomApp.Data
{
    class MongoDbInstance
    {
        private static MongoDbInstance? instance;

        public static MongoDbInstance Instance
        {
            get
            {
                instance ??= new MongoDbInstance();
                return instance;
            }
        }

        public IMongoDatabase Database { get; private set; }

        private MongoDbInstance()
        {
            var client = new MongoClient(Properties.Settings.Default.MongoDbConnectionString);
            Database = client.GetDatabase("DevicesData");
        }

        public void SetDatabase(string connectionString)
        {
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase("DevicesData");
        }
    }
}
