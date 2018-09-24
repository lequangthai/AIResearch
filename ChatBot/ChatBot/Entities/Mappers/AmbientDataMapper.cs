using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Entities.Mappers
{
    public static class AmbientDataMapper
    {
        public static AmbientData MapFromQueryResponeItem(Dictionary<string, AttributeValue> item)
        {
            if (item.Any())
            {
                var ambientData = new AmbientData();
                foreach (var propertyValue in item)
                {
                    switch (propertyValue.Key)
                    {
                        case "devicename":
                            ambientData.DeviceName = propertyValue.Value.S;
                            break;
                        case "airquality":
                            ambientData.AirQuality = propertyValue.Value.S;
                            break;
                        case "brightness":
                            ambientData.Brightness = propertyValue.Value.S;
                            break;
                        case "humidity":
                            ambientData.Humidity = propertyValue.Value.S;
                            break;
                        case "noise":
                            ambientData.Noise = propertyValue.Value.S;
                            break;
                        case "temperature":
                            ambientData.Temperature = propertyValue.Value.S;
                            break;
                        case "tstamp":
                            ambientData.Tstamp = propertyValue.Value.S;
                            break;
                    }
                }

                return ambientData;
            }

            return null;
        }

        public static AmbientData MapFromQueryResponse(QueryResponse response)
        {
            if (response != null && response.Items.Any())
            {
                return response.Items.Select(item => MapFromQueryResponeItem(item)).Where(data => data != null).FirstOrDefault();
            }

            return null;
        }
    }
}