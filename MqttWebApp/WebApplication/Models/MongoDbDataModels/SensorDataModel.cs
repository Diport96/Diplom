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
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Value { get; set; }
        public string User { get; set; }
        public double GetValue => double.Parse(Value);
        public long DateToMilliseconds => (long)Date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; 
    }
}
