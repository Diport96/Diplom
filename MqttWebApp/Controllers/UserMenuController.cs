using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MqttWebApp.Data;
using MqttWebApp.Models.MongoDbDataModels;

namespace MqttWebApp.Controllers
{
    [Authorize]
    public class UserMenuController : Controller
    {
        private readonly IMongoDatabase database;

        public UserMenuController()
        {
            database = MongoDbInstance.Instance.Database;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetJsonData()
        {
            var user = User.Identity.Name;
            return Json(GetStatisticUserTermometersData(user, database).Result);
        }
        private async Task<List<object>> GetStatisticUserTermometersData(string user, IMongoDatabase database)
        {
            List<object> resultData = new List<object>
            {
                new[] {
                    new {
                    label = "Date",
                    type = "datetime"
                    },

                    new {
                    label = "Значение температуры",
                    type = "number"
                    }
                }
            };
            var collection = database.GetCollection<SensorDataModel>("Termometer", new MongoCollectionSettings());
            using (var cursor = await collection.FindAsync(x => x.User == user))
            {
                await cursor.ForEachAsync((x) =>
                {
                    resultData.Add(new dynamic[] { $"Date({x.DateToMilliseconds})", x.GetValue });
                });
            }
            return resultData;
        }
    }
}