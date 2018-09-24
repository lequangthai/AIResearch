using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Entities.Mappers
{
    public static class DevicesInLocationMapper
    {
        public static DevicesInLocation MapFromResponseItem(Dictionary<string, AttributeValue> item)
        {
            if (item.Any())
            {
                var deviceInLocation = new DevicesInLocation();
                foreach (var propertyValue in item)
                {
                    switch (propertyValue.Key)
                    {
                        case "location_id":
                            deviceInLocation.LocationId = propertyValue.Value.S;
                            break;
                        case "device_id":
                            deviceInLocation.DeviceId = propertyValue.Value.S;
                            break;
                        case "boardType":
                            deviceInLocation.BoardType = propertyValue.Value.S;
                            break;
                        case "deviceName":
                            deviceInLocation.DeviceName = propertyValue.Value.S;
                            break;
                        case "locationName":
                            deviceInLocation.LocationName = propertyValue.Value.S;
                            break;
                        case "room":
                            deviceInLocation.Room = propertyValue.Value.S;
                            break;
                        case "active":
                            deviceInLocation.Active = propertyValue.Value.BOOL;
                            break;
                    }
                }

                return deviceInLocation;
            }

            return null;
        }

        public static List<DevicesInLocation> MapFromResponse(QueryResponse response)
        {
            if (response != null && response.Items.Any())
            {
                return response.Items.Select(item => MapFromResponseItem(item)).Where(data => data != null).ToList();
            }

            return new List<DevicesInLocation>();
        }
    }
}