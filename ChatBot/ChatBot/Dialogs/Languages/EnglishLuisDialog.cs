using ChatBot.Interfaces;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    [LuisModel("f3d7423f-f7fe-4081-a199-a15bf1431948", "2dc8bd746a8a4ecab69062e177ea0c0f", domain: "southeastasia.api.cognitive.microsoft.com")]
    [Serializable]
    public class EnglishLuisDialog : AbstractLuisDialog, IEnglishDialog<object>
    {
        public EnglishLuisDialog(IDeviceDataService deviceDataService,
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

        public override Task GetHelp(IDialogContext context, LuisResult result)
        {
            
            return base.GetHelp(context, result);
        }
    }
}