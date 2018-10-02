using ChatBot.Models;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Ultilities.Instances
{
    public class MessageBuilderService : IMessageBuilderService
    {
        private const string BreakLine = "\n";
        private const string BreakParagraph = "\n +++ \n";
        private const string Undefined = "undefined";

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

        public string BuildHelpMessage(string originalQuery)
        {
            return $"Sorry, I did not understand '{originalQuery}'. Type 'help' if you need assistance.";
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
                    var locationHeader = $"{locationName} are: {BreakLine}";
                    if (!string.IsNullOrEmpty(commandInfo))
                    {
                        locationHeader = $"{commandInfo} of {locationHeader}";
                    }
                    result = result + locationHeader;

                    // build status-result of each room on location
                    var roomsInLocation = rooms.Where(c => c.LocationName == locationName).ToList();
                    roomsInLocation.ForEach(room =>
                    {
                        var ambientValue = string.Empty;
                        if (!string.IsNullOrEmpty(commandInfo))
                        {
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
                        }

                        if (!string.IsNullOrEmpty(ambientValue))
                        {
                            result += $"{ambientValue} at {room.RoomName}"
                                + (roomsInLocation.IndexOf(room) != (roomsInLocation.Count - 1) ? $", {BreakLine}" : BreakLine);
                        }
                        else
                        {
                            if (!IsNullOrUndefined(room.Data.AirQuality))
                            {
                                result += $"air at {room.RoomName} is: {room.Data.AirQuality}";
                            }
                            if (!IsNullOrUndefined(room.Data.Brightness))
                            {
                                result += $",{BreakLine} birght at {room.RoomName} is: {room.Data.Brightness}";
                            }
                            if (!IsNullOrUndefined(room.Data.Humidity))
                            {
                                result += $",{BreakLine} humidity at {room.RoomName} is: {room.Data.Humidity}";
                            }
                            if (!IsNullOrUndefined(room.Data.Noise))
                            {
                                result += $",{BreakLine} noise at {room.RoomName} is: {room.Data.Noise}";
                            }
                            if (!IsNullOrUndefined(room.Data.Temperature))
                            {
                                result += $",{BreakLine} temperature at {room.RoomName} is: {room.Data.Temperature}";
                            }
                        }
                        if (roomsInLocation.IndexOf(room) < (roomsInLocation.Count - 1))
                        {
                            result += BreakParagraph;
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

        private bool IsNullOrUndefined(string str)
        {
            return string.IsNullOrEmpty(str) || str.Equals(Undefined);
        }
    }
}