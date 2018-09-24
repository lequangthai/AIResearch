using ChatBot.Ultilities.Interfaces;
using ChatBot.Ultilities.PiplineProcess.ProccessMessage;

namespace ChatBot.Ultilities.Instances
{
    public class ProccessMessageService : IProccessMessageService
    {
        private IDeviceDataService _deviceDataService;
        private ISpellCheckService _spellCheckService;
        private ITranslationService _translationService;

        public ProccessMessageService(IDeviceDataService deviceDataService,
            ISpellCheckService spellCheckService,
            ITranslationService translationService)
        {
            _deviceDataService = deviceDataService;
            _spellCheckService = spellCheckService;
            _translationService = translationService;
        }

        public string PipelineProccessGetData(string message)
        {
            var proccessMessagePipelines = new ProccessMessagePipline();

            proccessMessagePipelines
                .Register(new SpellCheckProccess(_spellCheckService))
                .Register(new DeviceDataCollectProccess(_deviceDataService));

            return proccessMessagePipelines.Process(message);
        }

        public string PipelineProccessTranslate(string message)
        {
            var proccessMessagePipelines = new ProccessMessagePipline();

            proccessMessagePipelines
                .Register(new SpellCheckProccess(_spellCheckService))
                .Register(new TranslateProccess(_translationService));

            return proccessMessagePipelines.Process(message);
        }
    }
}