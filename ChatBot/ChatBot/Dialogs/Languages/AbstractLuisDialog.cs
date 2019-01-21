using ChatBot.Models;
using ChatBot.Ultilities;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    [Serializable]
    public abstract class AbstractLuisDialog : LuisDialog<UserSelectedData>
    {
        public const string EntityKeyInfo = "keyInfo";
        public const string EntityLocation = "location";
        public const string EntityRoomName = "roomKey";

        public IDeviceDataService DeviceDataService;
        public ISpellCheckService SpellCheckService;
        public IMessageBuilderService MessageBuilderService;

        public AbstractLuisDialog(IDeviceDataService deviceDataService,
            ISpellCheckService spellCheckService,
            IMessageBuilderService messageBuilderService)
        {
            DeviceDataService = deviceDataService;
            SpellCheckService = spellCheckService;
            MessageBuilderService = messageBuilderService;
        }

        public override Task StartAsync(IDialogContext context)
        {
            context.UserData.SetValue(AppConstants.UserSelectedDataKey, new UserSelectedData());
            context.Wait(MessageReceived);
            return Task.CompletedTask;
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public virtual Task None(IDialogContext context, LuisResult result)
        {
            context.Wait(MessageReceived);
            return Task.CompletedTask;
        }

        [LuisIntent("GetCurrentStatus")]
        public virtual Task GetLocationStatus(IDialogContext context, LuisResult result)
        {
            ProccessLUISResultToContext(context, result);
            ProccessGetCurrentStatusMessage(context);
            return Task.CompletedTask;
        }

        [LuisIntent("Help")]
        public virtual Task GetHelp(IDialogContext context, LuisResult result)
        {
            context.Wait(MessageReceived);
            return Task.CompletedTask;
        }

        [LuisIntent("Greeting")]
        public virtual Task GetGreetingMessage(IDialogContext context, LuisResult result)
        {
            context.Wait(MessageReceived);
            return Task.CompletedTask;
        }

        #region Ultility methods

        public virtual Task PostMessage(IDialogContext context, string message)
        {
            context.PostAsync(message);
            return Task.CompletedTask;
        }

        public virtual void ProccessLUISResultToContext(IDialogContext context, LuisResult result)
        {
            (EntityRecommendation keyInfoEntityRecommendation,
                EntityRecommendation locationEntityRecommendation,
                EntityRecommendation roomNameRecommendation) = TryParseLUISResult(result);

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
            result.TryFindEntity(EntityKeyInfo, out EntityRecommendation keyInfoEntityRecommendation);
            result.TryFindEntity(EntityLocation, out EntityRecommendation locationEntityRecommendation);
            result.TryFindEntity(EntityRoomName, out EntityRecommendation roomNameRecommendation);

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

        private async Task ProccessGetCurrentStatusMessage(IDialogContext context)
        {
            var userSelectedData = GetUserSelectedData(context);
            if (!IsValidUserSelectedData(userSelectedData, context)) return;

            var waitingMessage = MessageBuilderService.ProccessWaitingMessage(userSelectedData);
            await PostMessage(context, waitingMessage);

            if (userSelectedData.IsHasKeyInfo || userSelectedData.IsHasLocationName || userSelectedData.IsHasRoomName)
            {
                var message = DeviceDataService.GetCurrentStatus(userSelectedData);

                SetUserSelectedData(context, new UserSelectedData());
                await PostMessage(context, message);
            }

            context.Wait(this.MessageReceived);
        }

        private UserSelectedData GetUserSelectedData(IDialogContext context)
        {
            UserSelectedData userSelectedData = null;
            context.UserData.TryGetValue(AppConstants.UserSelectedDataKey, out userSelectedData);
            if (userSelectedData == null)
            {
                userSelectedData = new UserSelectedData();
            }

            return userSelectedData;
        }

        private void SetUserSelectedData(IDialogContext context, UserSelectedData userSelectedData)
        {
            context.UserData.SetValue(AppConstants.UserSelectedDataKey, userSelectedData);
        }

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

        private async Task ProccessGreetingMessage(IDialogContext context)
        {
            var greetingMessage = MessageBuilderService.BuildGreetingMessage();
            await PostMessage(context, greetingMessage);
        }

        #endregion
    }
}