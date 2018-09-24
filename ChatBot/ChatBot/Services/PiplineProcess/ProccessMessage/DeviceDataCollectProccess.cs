using ChatBot.Ultilities.Interfaces;
using ChatBot.Ultilities.PiplineProcess.Base;

namespace ChatBot.Ultilities.PiplineProcess.ProccessMessage
{
    public class DeviceDataCollectProccess : IPipelineElement<string>
    {
        private IDeviceDataService _deviceDataService;

        public DeviceDataCollectProccess(IDeviceDataService deviceDataService)
        {
            _deviceDataService = deviceDataService;
        }

        public string Execute(string input)
        {
            return _deviceDataService.GetInfoByLocation(input);
        }
    }
}