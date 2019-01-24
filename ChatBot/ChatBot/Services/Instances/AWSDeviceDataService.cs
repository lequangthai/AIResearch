using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using ChatBot.Entities;
using ChatBot.Entities.Mappers;
using ChatBot.Models;
using ChatBot.Models.Mappers;
using ChatBot.Ultilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.Ultilities.Instances
{
    public class AWSDeviceDataService : IDeviceDataService
    {
        private readonly IMessageBuilderService _messageBuilderService;

        public AWSDeviceDataService(IMessageBuilderService messageBuilderService)
        {
            _messageBuilderService = messageBuilderService;
        }

        #region GetCurrentStatus
        public string GetCurrentStatus(UserSelectedData userSelectedData)
        {
            try
            {
                var client = GetDbClient();
                
                // get device of location
                var deviceInLocations = GetDevicesInLocation(client, userSelectedData);

                if (deviceInLocations.Any())
                {
                    var rooms = QueryInformationOfRooms(client, deviceInLocations);
                    return _messageBuilderService.BuildStatusInfoMessage(userSelectedData.KeyInfo, rooms);
                }
                else
                {
                    return _messageBuilderService.BuildNotFoundMessage(userSelectedData);
                }
            }
            catch (Exception ex)
            {

            }

            return Resources.Resource.HelpWithInstruction;
        }

        private List<DevicesInLocation> GetDevicesInLocation(AmazonDynamoDBClient client, UserSelectedData userSelectedData)
        {
            List<DevicesInLocation> devicesInLocations = new List<DevicesInLocation>();
            if (userSelectedData.IsHasLocationName)
            {
                var keyConditionExpression = "location_id = :v_locationName";
                var expressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_locationName", new AttributeValue { S = userSelectedData.LocationName } }
                };

                var request = new QueryRequest
                {
                    TableName = "devices-in-location",
                    KeyConditionExpression = keyConditionExpression,
                    ExpressionAttributeValues = expressionAttributeValues
                };

                var response = client.Query(request);
                devicesInLocations = DevicesInLocationMapper.MapFromResponse(response);
                if (userSelectedData.IsHasRoomName)
                {
                    devicesInLocations = devicesInLocations.Where(c => c.Room.Equals(userSelectedData.RoomName)).ToList();
                }
            }
            
            return devicesInLocations;
        }

        private List<RoomDisplayModel> QueryInformationOfRooms(AmazonDynamoDBClient client, List<DevicesInLocation> devicesInLocations)
        {
            var result = new List<RoomDisplayModel>();

            var deviceIds = devicesInLocations.Select(c => c.DeviceId).ToList();
            var lastUpdates = GetLastUpdates(client, deviceIds);
            if (lastUpdates.Any())
            {
                var ambientDatas = GetAmbientDataOfRooms(client, lastUpdates);

                result = RoomDisplayModelMapper.MapFromEntities(ambientDatas, devicesInLocations);
            }
            return result;
        }

        private List<AmbientData> GetAmbientDataOfRooms(AmazonDynamoDBClient client, List<LastUpdate> lastUpdates)
        {
            var tasks = new List<Task<AmbientData>>();
            lastUpdates.ForEach(lastUpdate =>
            {
                tasks.Add(GetAmbientDataOfRoom(client, lastUpdate));
            });

            Task.WaitAll(tasks.ToArray());
            var ambientDatas = new List<AmbientData>();
            tasks.ForEach(task =>
            {
                ambientDatas.Add(task.Result);
            });

            return ambientDatas;
        }

        private Task<AmbientData> GetAmbientDataOfRoom(AmazonDynamoDBClient client, LastUpdate lastUpdate)
        {
            var request = new QueryRequest
            {
                TableName = "ambient-data",
                KeyConditionExpression = "devicename = :v_devicename and tstamp = :v_tstamp",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {  ":v_devicename", new AttributeValue { S = lastUpdate.DeviceName } },
                    {  ":v_tstamp", new AttributeValue { S = lastUpdate.Tstamp } }
                }
            };

            var response = client.Query(request);
            var ambientData = AmbientDataMapper.MapFromQueryResponse(response);

            return Task.FromResult(ambientData);
        }

        private List<LastUpdate> GetLastUpdates(AmazonDynamoDBClient client, List<string> deviceIds)
        {
            var request = new ScanRequest
            {
                TableName = "lastupdate",
                ScanFilter = new Dictionary<string, Condition>
                    {
                        { "devicename", new Condition()
                            {
                                ComparisonOperator = ComparisonOperator.IN,
                                AttributeValueList = deviceIds.Select(x => new AttributeValue { S = x }).ToList()
                            }
                        }
                    }
            };
            var response = client.Scan(request);
            var result = LastUpdateMapper.MapFromResponse(response);

            return result;
        }

        private AmazonDynamoDBClient GetDbClient()
        {
            var region = RegionEndpoint.GetBySystemName("eu-central-1");
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(
                awsAccessKeyId: "AKIAJIT46T2TTGHNN3FA",
                awsSecretAccessKey: "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", region: region);
            
            return client;
        }
        #endregion

        #region GetInfoByLocation
        public string GetInfoByLocation(string message)
        {
            try
            {
                var commandInfo = message.Substring(0, message.IndexOf(' '));
                var locationId = message.Substring(message.LastIndexOf(' ') + 1);

                var client = GetDbClient();
                var context = new DynamoDBContext(client);

                // get device of location
                var deviceInLocations = context.Query<DevicesInLocation>(locationId).ToList();

                if (deviceInLocations.Any())
                {
                    return GetMessageInfo(client, context, commandInfo, deviceInLocations);
                }
            }
            catch (Exception ex)
            {

            }

            return string.Empty;
        }

        private string GetMessageInfo(AmazonDynamoDBClient client, DynamoDBContext context,
           string commandInfo, List<DevicesInLocation> deviceInLocations)
        {
            var result = $"{deviceInLocations.First().LocationName} is: \n";
            if (!string.IsNullOrEmpty(commandInfo))
            {
                result = $"{commandInfo} of {result}";
            }

            deviceInLocations.ForEach(x =>
            {
                var item = GetInformationOfRoom(client, context, x);
                if (item.data != null)
                {
                    var ambientValue = "undefined";
                    switch (commandInfo)
                    {
                        case "air":
                            ambientValue = item.data.AirQuality;
                            break;
                        case "light":
                            ambientValue = item.data.Brightness;
                            break;
                        case "humidity":
                            ambientValue = item.data.Humidity;
                            break;
                        case "noise":
                            ambientValue = item.data.Noise;
                            break;
                        case "temp":
                            ambientValue = item.data.Temperature;
                            break;
                    }

                    result += $"{ambientValue} at {item.roomName}"
                            + (deviceInLocations.IndexOf(x) != (deviceInLocations.Count - 1) ? ", " : string.Empty);
                }
            });

            return result;
        }

        private (string roomName, AmbientData data) GetInformationOfRoom(AmazonDynamoDBClient client, DynamoDBContext context,
            DevicesInLocation devicesInLocation)
        {
            var lastUpdate = GetLastUpdate(client, devicesInLocation.DeviceId);
            if (lastUpdate != null)
            {
                var deviceInfos = context.Query<AmbientData>(lastUpdate.DeviceName, QueryOperator.Equal, lastUpdate.Tstamp).ToList();
                if (deviceInfos.Any())
                {
                    return (devicesInLocation.Room, deviceInfos.First());
                }
            }
            return (devicesInLocation.Room, null);
        }

        private LastUpdate GetLastUpdate(AmazonDynamoDBClient client, string deviceId)
        {
            var request = new QueryRequest
            {
                TableName = "lastupdate",
                KeyConditions = new Dictionary<string, Condition>
                    {
                        { "devicename", new Condition()
                            {
                                ComparisonOperator = ComparisonOperator.EQ,
                                AttributeValueList = new List<AttributeValue>
                                {
                                    new AttributeValue { S = deviceId }
                                }
                            }
                        }
                    }
            };
            var response = client.Query(request);
            if (response != null && response.Items.Any())
            {
                var lastUpdate = new LastUpdate();

                var item = response.Items.First();
                foreach (var propertyValue in item)
                {
                    switch (propertyValue.Key)
                    {
                        case "devicename":
                            lastUpdate.DeviceName = propertyValue.Value.S;
                            break;
                        case "topic":
                            lastUpdate.Topic = propertyValue.Value.S;
                            break;
                        case "tstamp":
                            lastUpdate.Tstamp = propertyValue.Value.S;
                            break;
                    }
                }
                return lastUpdate;
            }
            return null;
        }
        #endregion
    }
}