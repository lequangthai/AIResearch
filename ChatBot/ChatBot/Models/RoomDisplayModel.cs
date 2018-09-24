using ChatBot.Entities;
using System.Collections.Generic;

namespace ChatBot.Models
{
    public class RoomDisplayModel
    {
        public string DeviceId { get; set; }

        public string LocationName { get; set; }

        public string RoomName { get; set; }

        public AmbientData Data { get; set; }
    }
}