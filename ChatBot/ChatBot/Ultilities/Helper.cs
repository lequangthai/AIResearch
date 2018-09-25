using System;
using System.Linq;

namespace ChatBot.Ultilities
{
    public static class Helper
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static bool IsContainsValue(Type type, string strValue)
        {
            return Enum.GetNames(type).Contains(strValue);
        }
    }
}