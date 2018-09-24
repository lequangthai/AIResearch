using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class SelectDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            PromptChoicetDialog.Choice(context, ReturnSelection, BuildOptions(), BuildPromptText(), BuildRetryText(), BuildAttemptsCount());
        }

        protected virtual async Task ReturnSelection(IDialogContext context, IAwaitable<string> input)
        {
            var selection = await input;

            context.Done(selection);
        }

        public virtual List<string> BuildOptions()
        {
            return new List<string>();
        }

        public virtual string BuildPromptText()
        {
            return "prompt text";
        }

        public virtual string BuildRetryText()
        {
            return "retry text";
        }

        public virtual int BuildAttemptsCount()
        {
            return 2;
        }
    }
}