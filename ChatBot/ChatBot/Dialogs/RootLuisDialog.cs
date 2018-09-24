using System;
using System.Threading.Tasks;
using ChatBot.Models;
using ChatBot.Ultilities;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace ChatBot.Dialogs
{
    [LuisModel("f3d7423f-f7fe-4081-a199-a15bf1431948", "2dc8bd746a8a4ecab69062e177ea0c0f", domain: "southeastasia.api.cognitive.microsoft.com")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<UserSelectedData>
    {
        private const string EntityKeyInfo = "keyInfo";
        private const string EntityLocation = "location";
        private const string EntityRoomName = "roomKey";

        private IDeviceDataService _deviceDataService;
        private ISpellCheckService _spellCheckService;
        private IMessageBuilderService _messageBuilderService;

        public RootLuisDialog(IDeviceDataService deviceDataService,
            ISpellCheckService spellCheckService,
            IMessageBuilderService messageBuilderService)
        {
            _deviceDataService = deviceDataService;
            _spellCheckService = spellCheckService;
            _messageBuilderService = messageBuilderService;
        }

        public async Task StartAsync(IDialogContext context)
        {
            SetUserSelectedData(context, new UserSelectedData());
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await PostMessage(context, message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("GetCurrentStatus")]
        public async Task GetLocationStatus(IDialogContext context, LuisResult result)
        {
            TryProccessLUISResultToUserSelectedData(context, result);
            await ProccessGetCurrentStatusMessage(context);
        }

        [LuisIntent("Help")]
        public async Task GetHelp(IDialogContext context, LuisResult result)
        {

        }

        [LuisIntent("Greeting")]
        public async Task GetGreetingMessage(IDialogContext context, LuisResult result)
        {
            await ProccessGreetingMessage(context);
        }

        private async Task ProccessGetCurrentStatusMessage(IDialogContext context)
        {
            var userSelectedData = GetUserSelectedData(context);
            if (!userSelectedData.IsHasKeyInfo)
            {
                var dialog = new KeyInfoSelectDialog();
                context.Call(dialog, SelectedKeyInfoProccess);

                return;
            }

            var waitingMessage = _messageBuilderService.ProccessWaitingMessage(userSelectedData);
            await PostMessage(context, waitingMessage);

            if (userSelectedData.IsHasKeyInfo || userSelectedData.IsHasLocationName || userSelectedData.IsHasRoomName)
            {
                await PostMessage(context, _deviceDataService.GetCurrentStatus(userSelectedData));
            }

            context.Wait(this.MessageReceived);
        }

        private async Task SelectedKeyInfoProccess(IDialogContext context, IAwaitable<string> input)
        {
            var selection = await input;

            if (!string.IsNullOrEmpty(selection))
            {
                var userSelectedData = GetUserSelectedData(context);
                userSelectedData.KeyInfo = selection;
                SetUserSelectedData(context, userSelectedData);

                await ProccessGetCurrentStatusMessage(context);
            }
            else
            {
                // Cancel
                SetUserSelectedData(context, new UserSelectedData());
                await ProccessGreetingMessage(context);
            }
        }

        private async Task ProccessGreetingMessage(IDialogContext context)
        {
            var greetingMessage = _messageBuilderService.BuildGreetingMessage();
            await PostMessage(context, greetingMessage);
        }

        #region Ultility methods
        private async Task PostMessage(IDialogContext context, string message)
        {
            await context.PostAsync(message);
        }

        private UserSelectedData GetUserSelectedData(IDialogContext context)
        {
            UserSelectedData userSelectedData = null;
            context.UserData.TryGetValue<UserSelectedData>(Constant.UserSelectedDataKey, out userSelectedData);
            if (userSelectedData == null)
            {
                userSelectedData = new UserSelectedData();
            }

            return userSelectedData;
        }

        private void SetUserSelectedData(IDialogContext context, UserSelectedData userSelectedData)
        {
            context.UserData.SetValue<UserSelectedData>(Constant.UserSelectedDataKey, userSelectedData);
        }

        private void TryProccessLUISResultToUserSelectedData(IDialogContext context, LuisResult result)
        {
            (EntityRecommendation keyInfoEntityRecommendation, EntityRecommendation locationEntityRecommendation, EntityRecommendation roomNameRecommendation) = TryParseLUISResult(result);

            var userSelectedData = GetUserSelectedData(context);
            userSelectedData.LastInputMessage = result.Query;
            if (keyInfoEntityRecommendation != null)
            {
                userSelectedData.KeyInfo = keyInfoEntityRecommendation.Entity;
            }
            if (locationEntityRecommendation != null)
            {
                userSelectedData.LocationName = locationEntityRecommendation.Entity;
            }
            if (roomNameRecommendation != null)
            {
                userSelectedData.RoomName = roomNameRecommendation.Entity;
            }

            SetUserSelectedData(context, userSelectedData);
        }

        private (EntityRecommendation keyInfoEntityRecommendation, EntityRecommendation locationEntityRecommendation, EntityRecommendation roomNameRecommendation)
            TryParseLUISResult(LuisResult result)
        {
            EntityRecommendation keyInfoEntityRecommendation = null;
            EntityRecommendation locationEntityRecommendation = null;
            EntityRecommendation roomNameRecommendation = null;

            result.TryFindEntity(EntityKeyInfo, out keyInfoEntityRecommendation);
            result.TryFindEntity(EntityLocation, out locationEntityRecommendation);
            result.TryFindEntity(EntityRoomName, out roomNameRecommendation);

            if (locationEntityRecommendation != null)
            {
                locationEntityRecommendation.Entity = locationEntityRecommendation.Entity.FirstCharToUpper();
            }
            if (roomNameRecommendation != null)
            {
                roomNameRecommendation.Entity = roomNameRecommendation.Entity.FirstCharToUpper();
            }

            return (keyInfoEntityRecommendation, locationEntityRecommendation, roomNameRecommendation);
        }
        #endregion
    }
}