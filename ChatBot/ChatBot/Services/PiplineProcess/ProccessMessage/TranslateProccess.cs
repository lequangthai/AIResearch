using ChatBot.Ultilities.Interfaces;
using ChatBot.Ultilities.PiplineProcess.Base;

namespace ChatBot.Ultilities.PiplineProcess.ProccessMessage
{
    public class TranslateProccess : IPipelineElement<string>
    {
        private ITranslationService _translationService;

        public TranslateProccess(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        public string Execute(string input)
        {
            return _translationService.TranslateText(input);
        }
    }
}