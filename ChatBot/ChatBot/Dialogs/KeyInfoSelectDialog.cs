using ChatBot.Ultilities;
using System;
using System.Collections.Generic;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class KeyInfoSelectDialog : SelectDialog
    {
        public override void BuildSelectOptionArray()
        {
            var descs = new List<string>();
            var enumValues = typeof(KeyInfoValueEnum).GetEnumValues();
            foreach (KeyInfoValueEnum enumValue in enumValues)
            {
                descs.Add(enumValue.GetDescription());
            }

            SelectOptions = descs.ToArray();
        }

        public override string BuildPromptText()
        {
            return "Sorry, but I cannot understand what information you want to get, so please select the type of information you want know or type Cancel to make another query:";
        }
    }
}