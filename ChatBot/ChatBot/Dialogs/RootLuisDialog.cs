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
            string message = _messageBuilderService.BuildHelpMessage(result.Query);

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
            if (!IsValidUserSelectedData(userSelectedData, context)) return;

            var waitingMessage = _messageBuilderService.ProccessWaitingMessage(userSelectedData);
            await PostMessage(context, waitingMessage);

            if (userSelectedData.IsHasKeyInfo || userSelectedData.IsHasLocationName || userSelectedData.IsHasRoomName)
            {
                var message = _deviceDataService.GetCurrentStatus(userSelectedData);

                SetUserSelectedData(context, new UserSelectedData());
                await PostMessage(context, message);
            }

            context.Wait(this.MessageReceived);
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

        #region Validate UserSelectedData for get data
        private bool IsValidUserSelectedData(UserSelectedData userSelectedData, IDialogContext context)
        {
            if (!userSelectedData.IsHasKeyInfo)
            {
                var dialog = new KeyInfoSelectDialog();
                context.Call(dialog, SelectedKeyInfoProccess);

                return false;
            }

            if (!userSelectedData.IsHasLocationName)
            {
                var dialog = new LocationSelectDialog();
                context.Call(dialog, SelectedLocationProccess);

                return false;
            }

            return true;
        }

        private async Task SelectedKeyInfoProccess(IDialogContext context, IAwaitable<string> input)
        {
            await SelectedDataProccess(context, input, nameof(UserSelectedData.KeyInfo));
        }

        private async Task SelectedLocationProccess(IDialogContext context, IAwaitable<string> input)
        {
            await SelectedDataProccess(context, input, nameof(UserSelectedData.LocationName));
        }

        private async Task SelectedDataProccess(IDialogContext context, IAwaitable<string> input, string propertyName)
        {
            var selection = await input;

            if (!string.IsNullOrEmpty(selection))
            {
                var userSelectedData = GetUserSelectedData(context);
                typeof(UserSelectedData).GetProperty(propertyName).SetValue(userSelectedData, selection);
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
        #endregion
    }
}