using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class SelectDialog : IDialog<string>
    {
        protected Array SelectOptions;

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
            var result = new List<string>();

            BuildSelectOptionArray();
            if (SelectOptions != null)
            {
                foreach (var value in SelectOptions)
                {
                    result.Add(value.ToString());
                }
            }

            return result;
        }

        public virtual void BuildSelectOptionArray()
        {
            SelectOptions = new string[0];
        }

        public virtual string BuildPromptText()
        {
            return "Sorry, but I cannot understand information you want get, so please select your choice or type Cancel to make another query:";
        }

        public virtual string BuildRetryText()
        {
            return "Sorry but we still cannot get your selected, please help us choice again or type Cancel to make another query:";
        }

        public virtual int BuildAttemptsCount()
        {
            return 2;
        }
    }
}