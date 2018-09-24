using ChatBot.Ultilities;
using System;
using System.Collections.Generic;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class KeyInfoSelectDialog : SelectDialog
    {
        public override List<string> BuildOptions()
        {
            var result = new List<string>();

            var values = Enum.GetValues(typeof(KeyInfoValueEnum));
            foreach(var value in values)
            {
                result.Add(value.ToString());
            }

            return result;
        }

        public override string BuildPromptText()
        {
            return "Sorry, but I cannot understand information you want get, so please select your choice or type Cancel to make another query:";
        }

        public override string BuildRetryText()
        {
            return "Sorry but we still cannot get your selected, please help us choice again or type Cancel to make another query:";
        }
    }
}