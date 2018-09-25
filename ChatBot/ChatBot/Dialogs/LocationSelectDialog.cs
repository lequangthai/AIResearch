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
    }
}