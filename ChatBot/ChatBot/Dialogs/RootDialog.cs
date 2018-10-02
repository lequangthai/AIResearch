using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected IDeviceDataService _deviceDataService;
        protected ISpellCheckService _spellCheckService;
        protected IMessageBuilderService _messageBuilderService;

        public RootDialog(IDeviceDataService deviceDataService,
            ISpellCheckService spellCheckService,
            IMessageBuilderService messageBuilderService)
        {
            _deviceDataService = deviceDataService;
            _spellCheckService = spellCheckService;
            _messageBuilderService = messageBuilderService;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await ProccessMultiLanguageMessage(context, result);
            return;


            //var activity = await result as Activity;

            //var message = context.MakeMessage(); 
            //message.Text = activity.Text;

            //var dialog = new RoomAssiantENLuisDialog(_deviceDataService, _spellCheckService, _messageBuilderService);
            //await context.Forward(dialog, FormCompleteCallback, message, CancellationToken.None);
        }

        private async Task FormCompleteCallback(IDialogContext context, IAwaitable<object> input)
        {
            context.Done<object>(null);
        }

        private async Task ProccessMultiLanguageMessage(IDialogContext context, IAwaitable<object> result)
        {
            const string LCID = "LCID";

            var activity = await result as Activity;
            if (!context.PrivateConversationData.ContainsKey(LCID))
            {
                context.PrivateConversationData.SetValue(LCID, activity.Text);
            }
            else
            {
                var lcid = context.PrivateConversationData.GetValueOrDefault<string>(LCID);

                int length = (activity.Text ?? string.Empty).Length;
                await context.PostAsync($"You sent {activity.Text} which was {length} characters.  Your chosen language code is: {lcid}");
                await context.PostAsync($"{Resources.Resource.String1}");

                context.Wait(MessageReceivedAsync);
            }
        }
    }
}