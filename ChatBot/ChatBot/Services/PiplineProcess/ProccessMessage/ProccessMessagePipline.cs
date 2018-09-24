using ChatBot.Ultilities.PiplineProcess.Base;

namespace ChatBot.Ultilities.PiplineProcess.ProccessMessage
{
    public class ProccessMessagePipline : Pipeline<string>
    {
        public override string Process(string input)
        {
            foreach (var pipe in pipeItems)
            {
                input = pipe.Execute(input);
            }

            return input;
        }
    }
}