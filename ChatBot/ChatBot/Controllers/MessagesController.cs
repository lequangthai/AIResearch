using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
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
                    // activity.Text = _proccessMessageService.PipelineProccessTranslate(activity.Text);
                    
                    // use autofact to resolve IDialog
                    using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                    {
                        var botData = scope.Resolve<IBotData>();
                        await botData.LoadAsync(CancellationToken.None);

                        var lcid = botData.PrivateConversationData.GetValueOrDefault<string>("LCID");
                        if (!string.IsNullOrEmpty(lcid))
                        {
                            activity.Locale = lcid;
                        }

                        await botData.FlushAsync(CancellationToken.None);

                        await Conversation.SendAsync(activity, () => scope.Resolve<IDialog<object>>());
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