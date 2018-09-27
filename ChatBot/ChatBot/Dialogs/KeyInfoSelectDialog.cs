using ChatBot.Ultilities;
using System;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class KeyInfoSelectDialog : SelectDialog
    {
        public override void BuildSelectOptionArray()
        {
            SelectOptions = Enum.GetValues(typeof(KeyInfoValueEnum));
        }

        public override string BuildPromptText()
        {
            return "Sorry, but I cannot understand what information you want to get, so please select the type of information you want know or type Cancel to make another query:";
        }
    }
}