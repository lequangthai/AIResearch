using ChatBot.Interfaces;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    [LuisModel("0d1438e2-451a-4bee-9a24-40edffb356e0", "e6503e8c566643749f096dc32d970439", domain: "southeastasia.api.cognitive.microsoft.com")]
    [Serializable]
    public class GermanLuisDialog : AbstractLuisDialog, IGermanDialog<object>
    {
        public GermanLuisDialog(IDeviceDataService deviceDataService,
           ISpellCheckService spellCheckService,
           IMessageBuilderService messageBuilderService)
           : base(deviceDataService, spellCheckService, messageBuilderService) { }

        public override Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";
            PostMessage(context, message).Wait();
            return base.None(context, result);
        }

        public override Task GetGreetingMessage(IDialogContext context, LuisResult result)
        {
            var message = MessageBuilderService.BuildGreetingMessage();
            PostMessage(context, message).Wait();
            return base.GetGreetingMessage(context, result);
        }
    }
}