using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MqttWebApp.Models.MongoDbDataModels
{
    public class SensorDataModel
    {
        public ObjectId id { get; set; }
        public string ID { get; set; }
        public DateTime Date { get; set; }
        public string Value { get; set; }
        public string User { get; set; }
        public double GetValue => double.Parse(Value);
    }
}
