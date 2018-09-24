using ChatBot.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Models.Mappers
{
    public static class RoomDisplayModelMapper
    {
        public static List<RoomDisplayModel> MapFromEntities(List<AmbientData> ambientDatas, List<DevicesInLocation> devicesInLocations)
        {
            var result = new List<RoomDisplayModel>();
            if (ambientDatas != null && ambientDatas.Any()
                && devicesInLocations != null && devicesInLocations.Any())
            {
                ambientDatas.ForEach(ambientData =>
                {
                    var deviceInLocation = devicesInLocations.FirstOrDefault(x => x.DeviceId == ambientData.DeviceName);
                    if (deviceInLocation != null)
                    {
                        var roomDisplay = result.FirstOrDefault(r => r.DeviceId.Equals(deviceInLocation.DeviceId));
                        if (roomDisplay == null)
                        {
                            roomDisplay = new RoomDisplayModel
                            {
                                DeviceId = ambientData.DeviceName,
                                LocationName = deviceInLocation.LocationName,
                                RoomName = deviceInLocation.Room,
                                Data = ambientData
                            };

                            result.Add(roomDisplay);
                        }
                    }
                });
            }

            return result;
        }
    }
}