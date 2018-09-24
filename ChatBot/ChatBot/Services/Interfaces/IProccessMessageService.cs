namespace ChatBot.Ultilities.Interfaces
{
    public interface IProccessMessageService
    {
        string PipelineProccessGetData(string message);

        string PipelineProccessTranslate(string message);
    }
}