using ChatBot.Interfaces;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    [LuisModel("a7be38d4-604b-4e03-82f7-211d82b461df", "840e23e376be4fb1929fba8385a06d1e", domain: "westus.api.cognitive.microsoft.com")]
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