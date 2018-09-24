using Amazon.DynamoDBv2.DataModel;
using System;

namespace ChatBot.Entities
{
    [DynamoDBTable("ambient-data")]
    public class AmbientData
    {
        [DynamoDBHashKey("devicename")]
        public string DeviceName { get; set; }

        [DynamoDBProperty("airquality")]
        public string AirQuality { get; set; }

        [DynamoDBProperty("brightness")]
        public string Brightness { get; set; }

        [DynamoDBProperty("humidity")]
        public string Humidity { get; set; }

        [DynamoDBProperty("noise")]
        public string Noise { get; set; }

        [DynamoDBProperty("temperature")]
        public string Temperature { get; set; }

        [DynamoDBRangeKey("tstamp")]
        public string Tstamp { get; set; }
    }
}