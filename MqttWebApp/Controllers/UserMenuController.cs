using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MqttWebApp.Controllers
{
    public class UserMenuController : Controller
    {
        private readonly IMongoDatabase database;

        public UserMenuController()
        {
            var client = new MongoClient("mongodb://localhost/DevicesData");
            database = client.GetDatabase("DevicesData");
        }

        public IActionResult Index()
        {
            var collection = database.GetCollection<BsonDocument>("Sensor");
            var filter = new BsonDocument();
            var data = collection.Find(filter).ToList();

            return View();
        }
    }
}