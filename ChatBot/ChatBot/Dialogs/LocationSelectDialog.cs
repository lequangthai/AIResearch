using ChatBot.Ultilities;
using System;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class LocationSelectDialog : SelectDialog
    {
        public override void BuildSelectOptionArray()
        {
            SelectOptions = Enum.GetValues(typeof(LocationValueEnum));
        }

        public override string BuildPromptText()
        {
            return "Sorry, but I cannot detect what location you want to get, so please select the location you want to know or type Cancel to make another query:";
        }
    }
}