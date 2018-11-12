using ChatBot.Interfaces;
using ChatBot.Models;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System;

namespace ChatBot.Dialogs
{
    [LuisModel("0d1438e2-451a-4bee-9a24-40edffb356e0", "2dc8bd746a8a4ecab69062e177ea0c0f", domain: "southeastasia.api.cognitive.microsoft.com")]
    [Serializable]
    public class GermanLuisDialog : AbstractLuisDialog, IGermanDialog<object>
    {
        public GermanLuisDialog(IDeviceDataService deviceDataService,
           ISpellCheckService spellCheckService,
           IMessageBuilderService messageBuilderService)
           : base(deviceDataService, spellCheckService, messageBuilderService) { }
    }
}