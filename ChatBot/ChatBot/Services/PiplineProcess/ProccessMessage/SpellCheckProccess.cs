using ChatBot.Ultilities.Interfaces;
using ChatBot.Ultilities.PiplineProcess.Base;

namespace ChatBot.Ultilities.PiplineProcess.ProccessMessage
{
    public class SpellCheckProccess : IPipelineElement<string>
    {
        private ISpellCheckService _spellCheckService;

        public SpellCheckProccess(ISpellCheckService spellCheckService)
        {
            _spellCheckService = spellCheckService;
        }

        public string Execute(string input)
        {
            return _spellCheckService.GetCorrectedTextAsync(input).Result;
        }
    }
}