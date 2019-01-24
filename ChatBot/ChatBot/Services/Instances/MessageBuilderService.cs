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

        public string BuildGreetingMessage()
        {
            return Resources.Resource.Greeting;
        }

        public string BuildHelpMessage(UserSelectedData userSelectedData)
        {
            if (!userSelectedData.IsHasKeyInfo && !userSelectedData.IsHasLocationName &&! userSelectedData.IsHasRoomName)
            {
                return Resources.Resource.HelpWithInstruction;
            }
            return string.Empty;
        }

        public string BuildNotFoundMessage(UserSelectedData userSelectedData)
        {
            (bool isHasLocation, string locationStr) = GetLocationString(userSelectedData);

            if (isHasLocation)
            {
                return string.Format(Resources.Resource.HelpNotFoundLocationInput, locationStr);
            }
            else
            {
                return Resources.Resource.HelpNotFoundLocation;
            }
        }

        public string BuildHelpMessage(string originalQuery)
        {
            return string.Format(Resources.Resource.HelpNotUnderstand, originalQuery);
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
                    var locationHeader = string.Format(Resources.Resource.AreDefine2Phrase, locationName, BreakLine);
                    if (!string.IsNullOrEmpty(commandInfo))
                    {
                        locationHeader = string.Format(Resources.Resource.OfDefine2Phrase, commandInfo, locationHeader);
                    }
                    result = result + locationHeader;

                    // build status-result of each room on location
                    var roomsInLocation = rooms.Where(c => c.LocationName == locationName).ToList();
                    roomsInLocation.ForEach(room =>
                    {
                        var ambientValue = string.Empty;
                        if (!string.IsNullOrEmpty(commandInfo))
                        {
                            var keyInfoCommand = commandInfo.GetEnumValue<KeyInfoValueEnum>();
                            switch (keyInfoCommand)
                            {
                                case KeyInfoValueEnum.Air:
                                    ambientValue = room.Data.AirQuality;
                                    break;
                                case KeyInfoValueEnum.Light:
                                    ambientValue = room.Data.Brightness;
                                    break;
                                case KeyInfoValueEnum.Humidity:
                                    ambientValue = room.Data.Humidity;
                                    break;
                                case KeyInfoValueEnum.Noise:
                                    ambientValue = room.Data.Noise;
                                    break;
                                case KeyInfoValueEnum.Temp:
                                    ambientValue = room.Data.Temperature;
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(ambientValue))
                        {
                            result += string.Format(Resources.Resource.AtDefine2Phrase, ambientValue, room.RoomName)
                                + (roomsInLocation.IndexOf(room) != (roomsInLocation.Count - 1) ? $", {BreakLine}" : BreakLine);
                        }
                        else
                        {
                            if (!IsNullOrUndefined(room.Data.AirQuality))
                            {
                                result += string.Format(Resources.Resource.AirValueDefine, room.RoomName, room.Data.AirQuality);
                            }
                            if (!IsNullOrUndefined(room.Data.Brightness))
                            {
                                result += string.Format(Resources.Resource.BrightValueDefine, BreakLine, room.RoomName, room.Data.Brightness);
                            }
                            if (!IsNullOrUndefined(room.Data.Humidity))
                            {
                                result += string.Format(Resources.Resource.HumidityValueDefine, BreakLine, room.RoomName, room.Data.Humidity);
                            }
                            if (!IsNullOrUndefined(room.Data.Noise))
                            {
                                result += string.Format(Resources.Resource.NoiseValueDefine, BreakLine, room.RoomName, room.Data.Noise);
                            }
                            if (!IsNullOrUndefined(room.Data.Temperature))
                            {
                                result += string.Format(Resources.Resource.TemperatureValueDefine, BreakLine, room.RoomName, room.Data.Temperature);
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
                return string.Format(Resources.Resource.LookForInforOfDefine, userSelectedData.KeyInfo, locationStr);
            }
            else
            {
                if (userSelectedData.IsHasKeyInfo || isHasLocation)
                {
                    if (userSelectedData.IsHasKeyInfo)
                    {
                        return string.Format(Resources.Resource.LookForDefine, userSelectedData.KeyInfo);
                    }
                    else
                    {
                        return string.Format(Resources.Resource.LookForCurrentDefine, locationStr);
                    }
                }
                else
                {
                    return string.Format(Resources.Resource.HelpNotUnderstand, userSelectedData.LastInputMessage);
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
                    locationStr = string.Format(Resources.Resource.InDefine2Phrase, userSelectedData.RoomName, userSelectedData.LocationName);
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
            const string Undefined = "undefined";
            return string.IsNullOrEmpty(str) || str.Equals(Undefined);
        }
    }
}