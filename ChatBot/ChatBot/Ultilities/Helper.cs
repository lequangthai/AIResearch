using ChatBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;

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

        public static string GetDescription(this Enum enumerationValue)
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        public static T GetEnumValue<T>(this string strDescription) where T : Enum
        {
            var enumValues = typeof(T).GetEnumValues();
            foreach (T enumValue in enumValues)
            {
                if (enumValue.GetDescription().Equals(strDescription, StringComparison.OrdinalIgnoreCase))
                {
                    return enumValue;
                }
            }

            throw new Exception($"Cannot found Enum value for '{strDescription}'");
        }

        public static UserSelectedData GetUserSelectedData(IDialogContext context)
        {
            UserSelectedData userSelectedData = null;
            context.UserData.TryGetValue(AppConstants.UserSelectedDataKey, out userSelectedData);
            if (userSelectedData == null)
            {
                userSelectedData = new UserSelectedData();
            }

            return userSelectedData;
        }
    }

    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resource;
        public LocalizedDescriptionAttribute(string resourceKey)
        {
            _resource = new ResourceManager(typeof(Resources.Resource));
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string displayName = _resource.GetString(_resourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", _resourceKey)
                    : displayName;
            }
        }
    }
}