using ChatBot.Models;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Ultilities.Instances
{
    public class MessageBuilderService : IMessageBuilderService
    {
        public string BuildGreetingMessage()
        {
            return $"Hello, I would like to provide some information about the enviroment status of working location for you, please let me know what info you need?";
        }

        public string BuildHelpMessage(UserSelectedData userSelectedData)
        {
            (bool isHasLocation, string locationStr) = GetLocationString(userSelectedData);

            if (isHasLocation)
            {
                return $"Sorry, I cannot find your input: '{locationStr}'. Type 'help' if you need assistance.";
            }
            else
            {
                return $"Sorry, I cannot find any location in your input. Type 'help' if you need assistance.";
            }
        }

        public string BuildStatusInfoMessage(string commandInfo, List<RoomDisplayModel> rooms)
        {
            var result = "";
            if (rooms.Any())
            {
                var locationNames = rooms.Select(c => c.LocationName).Distinct().ToList();
                locationNames.ForEach(locationName =>
                {
                    // build header-result for each location
                    var locationHeader = $"{locationName} are: \n";
                    if (!string.IsNullOrEmpty(commandInfo))
                    {
                        locationHeader = $"{commandInfo} of {locationHeader}";
                    }
                    result = result + locationHeader;

                    // build status-result of each room on location
                    var roomsInLocation = rooms.Where(c => c.LocationName == locationName).ToList();
                    roomsInLocation.ForEach(room =>
                    {
                        if (!string.IsNullOrEmpty(commandInfo))
                        {
                            var ambientValue = "undefined";
                            switch (commandInfo)
                            {
                                case "air":
                                    ambientValue = room.Data.AirQuality;
                                    break;
                                case "light":
                                    ambientValue = room.Data.Brightness;
                                    break;
                                case "humidity":
                                    ambientValue = room.Data.Humidity;
                                    break;
                                case "noise":
                                    ambientValue = room.Data.Noise;
                                    break;
                                case "temp":
                                    ambientValue = room.Data.Temperature;
                                    break;
                            }

                            result += $"{ambientValue} at {room.RoomName}"
                                    + (roomsInLocation.IndexOf(room) != (roomsInLocation.Count - 1) ? ", \n" : "\n");
                        }
                        else
                        {
                            result += $"air at {room.RoomName} is: {room.Data.AirQuality}"
                                    + (roomsInLocation.IndexOf(room) != (roomsInLocation.Count - 1) ? ", \n" : "\n");
                            result += $"light at {room.RoomName} is: {room.Data.Brightness}"
                                    + (roomsInLocation.IndexOf(room) != (roomsInLocation.Count - 1) ? ", \n" : "\n");
                            result += $"humidity at {room.RoomName} is: {room.Data.Humidity}"
                                    + (roomsInLocation.IndexOf(room) != (roomsInLocation.Count - 1) ? ", \n" : "\n");
                            result += $"noise at {room.RoomName} is: {room.Data.Noise}"
                                    + (roomsInLocation.IndexOf(room) != (roomsInLocation.Count - 1) ? ", \n" : "\n");
                            result += $"temp at {room.RoomName} is: {room.Data.Temperature}"
                                    + (roomsInLocation.IndexOf(room) != (roomsInLocation.Count - 1) ? ", \n" : "\n");
                        }
                    });
                });
            }

            return result;
        }

        public string ProccessWaitingMessage(UserSelectedData userSelectedData)
        {
            (bool isHasLocation, string locationStr) = GetLocationString(userSelectedData);

            if (userSelectedData.IsHasKeyInfo && isHasLocation)
            {
                return $"Looking for '{userSelectedData.KeyInfo}' of '{locationStr}'...";
            }
            else
            {
                if (userSelectedData.IsHasKeyInfo || isHasLocation)
                {
                    if (userSelectedData.IsHasKeyInfo)
                    {
                        return $"Looking for '{userSelectedData.KeyInfo}'...";
                    }
                    else
                    {
                        return $"Looking for current status of '{locationStr}'...";
                    }
                }
                else
                {
                    return $"Sorry, I did not understand '{userSelectedData.LastInputMessage}'. Type 'help' if you need assistance.";
                }
            }
        }

        private (bool isHasLocation, string locationStr) GetLocationString(UserSelectedData userSelectedData)
        {
            var isHasLocation = userSelectedData.IsHasLocationName || userSelectedData.IsHasRoomName;
            var locationStr = "";
            if (isHasLocation)
            {
                if (userSelectedData.IsHasLocationName && userSelectedData.IsHasRoomName)
                {
                    locationStr = $"{userSelectedData.RoomName} in {userSelectedData.LocationName}";
                }
                else
                {
                    locationStr = userSelectedData.IsHasLocationName ? userSelectedData.LocationName : userSelectedData.RoomName;
                }
            }

            return (isHasLocation, locationStr);
        }
    }
}