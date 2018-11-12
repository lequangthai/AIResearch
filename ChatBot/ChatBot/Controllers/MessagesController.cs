using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using ChatBot.Interfaces;
using ChatBot.Ultilities;
using ChatBot.Ultilities.Instances;
using ChatBot.Ultilities.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace ChatBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IProccessMessageService _proccessMessageService;
        public Dictionary<string, string> languages = new Dictionary<string, string>
        {
            { "en", AppConstants.EnglishLanguageCode},
            { "de", AppConstants.GermanLanguageCode},
            { "fr", AppConstants.FrenchLanguageCode}
        };

        public MessagesController()
        {
            _proccessMessageService = new ProccessMessageService(
                new AWSDeviceDataService(new MessageBuilderService()),
                new BingSpellCheckService(),
                new TranslationService());
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity.GetActivityType() == ActivityTypes.Message)
                {
                    var language = _proccessMessageService.PipelineProccessTranslate(activity.Text);

                    // use autofact to resolve IDialog
                    using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                    {
                        // Set language for Resources
                        var botData = scope.Resolve<IBotData>();
                        await botData.LoadAsync(CancellationToken.None);

                        languages.TryGetValue(language, out string languageValue);
                        botData.PrivateConversationData.SetValue(AppConstants.LCID, languageValue);

                        var LCID = botData.PrivateConversationData.GetValueOrDefault<string>(AppConstants.LCID);
                        if (!string.IsNullOrEmpty(LCID))
                        {
                            activity.Locale = LCID;
                        }

                        await botData.FlushAsync(CancellationToken.None);

                        // Re-direct to correct language class
                        switch (languageValue)
                        {
                            case AppConstants.GermanLanguageCode:
                                await Conversation.SendAsync(activity, () => scope.Resolve<IGermanDialog<object>>());
                                break;
                            case AppConstants.EnglishLanguageCode:
                                await Conversation.SendAsync(activity, () => scope.Resolve<IEnglishDialog<object>>());
                                break;
                        }
                    }
                }
                else
                {
                    HandleSystemMessage(activity);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);

                Trace.TraceError($"Response: {response.Content} - {response.StatusCode}");
                return response;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error: {ex.Message} - {ex.StackTrace}");
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            string messageType = message.GetActivityType();
            if (messageType == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (messageType == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (messageType == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (messageType == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (messageType == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}