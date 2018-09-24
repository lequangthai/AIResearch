using System;
using System.Threading.Tasks;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private IProccessMessageService _proccessMessageService;

        public RootDialog(IProccessMessageService proccessMessageService)
        {
            _proccessMessageService = proccessMessageService;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var ambientData = string.Empty;
            ambientData = _proccessMessageService.PipelineProccessGetData(activity.Text);

            if (!string.IsNullOrEmpty(ambientData))
            {
                // Return our reply to the user
                await context.PostAsync(ambientData);
            }
            else
            {
                // Calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                var message = $"You sent {activity.Text} which was {length} characters";
                await context.PostAsync(message);
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}