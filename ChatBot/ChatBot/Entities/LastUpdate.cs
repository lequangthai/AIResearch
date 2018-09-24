using Amazon.DynamoDBv2.DataModel;

namespace ChatBot.Entities
{
    [DynamoDBTable("lastupdate")]
    public class LastUpdate
    {
        [DynamoDBHashKey("devicename")]
        public string DeviceName { get; set; }

        [DynamoDBProperty("topic")]
        public string Topic { get; set; }

        [DynamoDBRangeKey("tstamp")]
        public string Tstamp { get; set; }
    }
}