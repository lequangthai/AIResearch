using Amazon.DynamoDBv2.DataModel;

namespace ChatBot.Entities
{
    [DynamoDBTable("devices-in-location")]
    public class DevicesInLocation
    {
        [DynamoDBHashKey("location_id")]
        public string LocationId { get; set; }

        [DynamoDBRangeKey("device_id")]
        public string DeviceId { get; set; }

        [DynamoDBProperty("boardType")]
        public string BoardType { get; set; }

        [DynamoDBProperty("deviceName")]
        public string DeviceName { get; set; }

        [DynamoDBProperty("locationName")]
        public string LocationName { get; set; }

        [DynamoDBProperty("room")]
        public string Room { get; set; }

        [DynamoDBProperty("active")]
        public bool Active { get; set; }
    }
}
