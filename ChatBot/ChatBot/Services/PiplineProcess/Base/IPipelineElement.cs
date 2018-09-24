namespace ChatBot.Ultilities.PiplineProcess.Base
{
    public interface IPipelineElement<T>
    {
        /// <summary>
        /// Filter implementing this method would perform processing on the input type T
        /// </summary>
        /// <param name="input">The input to be executed by the filter</param>
        /// <returns></returns>
        T Execute(T input);
    }
}
