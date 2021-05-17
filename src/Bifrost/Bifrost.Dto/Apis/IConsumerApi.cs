using System.Collections.Generic;
using Bifrost.Dto.Dto;

namespace Bifrost.Dto.Apis
{
    /// <summary>
    /// Provides and interface for handling consumers
    /// </summary>
    public interface IConsumerApi
    {
        /// <summary>
        /// Retrieves the consumers corresponding to the given names, or all if none are supplied
        /// </summary>
        /// <returns></returns>
        IList<ConsumerDto> GetConsumers(string[] consumerNames);

        /// <summary>
        /// Save or update the given consumer
        /// </summary>
        /// <param name="consumer"></param>
        void EditConsumer(ConsumerDto consumer);

        /// <summary>
        /// Delete consumer with the given name
        /// </summary>
        /// <param name="consumerName"></param>
        void DeleteConsumer(string consumerName);
    }
}
