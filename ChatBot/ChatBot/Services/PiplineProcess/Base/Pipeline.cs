using System.Collections.Generic;

namespace ChatBot.Ultilities.PiplineProcess.Base
{
    public abstract class Pipeline<T>
    {
        /// <summary>
        /// List of filters in the pipeline
        /// </summary>
        protected readonly List<IPipelineElement<T>> pipeItems = new List<IPipelineElement<T>>();

        /// <summary>
        /// To Register filter in the pipeline
        /// </summary>
        /// <param name="filter">A filter object implementing IFilter interface</param>
        /// <returns></returns>
        public Pipeline<T> Register(IPipelineElement<T> filter)
        {
            pipeItems.Add(filter);
            return this;
        }

        /// <summary>
        /// To start processing on the Pipeline
        /// </summary>
        /// <param name="input">
        /// The input object on which filter processing would execute</param>
        /// <returns></returns>
        public abstract T Process(T input);
    }
}