using System.Collections.Generic;
using Bifrost.Core.Domain;

namespace Bifrost.Core.ApplicationServices.Interfaces
{
    public interface IConsumerService
    {
        Consumer GetConsumerByName(string name);
        Consumer GetConsumerByApiKey(string apiKey);

        /// <summary>
        /// Get the first consumer that has the given job available to her
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        Consumer GetConsumerByJobName(string jobName);

        IList<Consumer> GetAllConsumers();

        void SaveOrUpdate(Consumer consumer);
        void DeleteConsumer(string name);

        
    }

}