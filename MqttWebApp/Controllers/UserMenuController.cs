using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MqttWebApp.Data;
using MqttWebApp.Models.MongoDbDataModels;

namespace MqttWebApp.Controllers
{
    public class UserMenuController : Controller
    {
        private readonly IMongoDatabase database;

        public UserMenuController()
        {
            database = MongoDbInstance.Instance.Database;
        }

        public async Task<IActionResult> Index()
        {
            List<SensorDataModel> sensorDataForCurrentMonth = new List<SensorDataModel>();
            var currentMonth = DateTime.Today.Month;
            var collection = database.GetCollection<SensorDataModel>("Sensor");
            string user = "Test@bk.ru";
            using (var cursor = await collection.FindAsync(x => x.User == user))
            {
                await cursor.ForEachAsync((x) =>
                {
                    if (x.Date.Month == currentMonth)
                        sensorDataForCurrentMonth.Add(x);
                });
            }

            return View();
        }
    }
}