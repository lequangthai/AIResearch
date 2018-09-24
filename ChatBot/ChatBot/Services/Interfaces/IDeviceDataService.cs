using ChatBot.Models;

namespace ChatBot.Ultilities.Interfaces
{
    public interface IDeviceDataService
    {
        string GetInfoByLocation(string message);

        string GetCurrentStatus(UserSelectedData userSelectedData);
    }
}