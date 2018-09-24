using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Entities.Mappers
{
    public static class LastUpdateMapper
    {
        public static LastUpdate MapFromResponseItem(Dictionary<string, AttributeValue> item)
        {
            if (item.Any())
            {
                var dbLastUpdateItem = new LastUpdate();
                foreach (var propertyValue in item)
                {
                    switch (propertyValue.Key)
                    {
                        case "devicename":
                            dbLastUpdateItem.DeviceName = propertyValue.Value.S;
                            break;
                        case "topic":
                            dbLastUpdateItem.Topic = propertyValue.Value.S;
                            break;
                        case "tstamp":
                            dbLastUpdateItem.Tstamp = propertyValue.Value.S;
                            break;
                    }
                }

                return dbLastUpdateItem;
            }

            return null;
        }

        public static List<LastUpdate> MapFromResponse(ScanResponse response)
        {
            if (response != null && response.Items.Any())
            {
                return response.Items.Select(item => MapFromResponseItem(item)).Where(data => data != null).ToList();
            }

            return new List<LastUpdate>();
        }
    }
}