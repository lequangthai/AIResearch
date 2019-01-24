using ChatBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;

namespace ChatBot.Ultilities.Interfaces
{
    public interface IMessageBuilderService
    {
        string BuildGreetingMessage();

        string BuildNotFoundMessage(UserSelectedData userSelectedData);

        string BuildStatusInfoMessage(string commandInfo, List<RoomDisplayModel> rooms);

        string ProccessWaitingMessage(UserSelectedData userSelectedData);

        string BuildHelpMessage(UserSelectedData userSelectedData);

        string BuildHelpMessage(string originalQuery);
    }
}
