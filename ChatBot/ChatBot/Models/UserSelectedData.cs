﻿using ChatBot.Ultilities;

namespace ChatBot.Models
{
    public class UserSelectedData
    {
        public string KeyInfo { get; set; }

        public bool IsHasKeyInfo
        {
            get
            {
                return !string.IsNullOrEmpty(KeyInfo) || Helper.IsContainsValue(typeof(KeyInfoValueEnum), KeyInfo);
            }
        }

        public string LocationName { get; set; }

        public bool IsHasLocationName
        {
            get
            {
                return !string.IsNullOrEmpty(LocationName) || Helper.IsContainsValue(typeof(LocationValueEnum), LocationName);
            }
        }

        public string RoomName { get; set; }

        public bool IsHasRoomName
        {
            get
            {
                return !string.IsNullOrEmpty(RoomName);
            }
        }

        public string LastInputMessage { get; set; }
    }
}