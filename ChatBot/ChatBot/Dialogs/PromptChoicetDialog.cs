using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class PromptChoicetDialog : PromptDialog.PromptChoice<string>
    {
        private static IEnumerable<string> cancelTerms = new List<string> { "Cancel", "Back", "B", "Abort" };

        public PromptChoicetDialog(List<string> options, string prompt, string retry, int attempt): base(options, prompt, retry, attempt)
        {

        }

        public static void Choice(IDialogContext context, ResumeAfter<string> resum, List<string> options, string prompt, string retry, int attempt)
        {
            var child = new PromptChoicetDialog(options, prompt, retry, attempt);
            context.Call(child, resum);
        }

        public static bool IsCancel(string text)
        {
            return cancelTerms.Any(t => string.Equals(t, text, StringComparison.CurrentCultureIgnoreCase));
        }

        protected override bool TryParse(IMessageActivity message, out string result)
        {
            if (IsCancel(message.Text))
            {
                result = string.Empty;
                return true;
            }

            return base.TryParse(message, out result);
        }
    }
}