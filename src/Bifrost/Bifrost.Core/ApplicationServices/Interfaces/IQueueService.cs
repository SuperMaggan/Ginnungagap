using System.Collections.Generic;
using Bifrost.Core.Domain;

namespace Bifrost.Core.ApplicationServices.Interfaces
{
    /// <summary>
    /// Provides an interface for handling queue items
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Pushes the given item to the queue stack of to the given category 
        /// Returns the number of new links that was present in the given items
        /// Will only add items with a documentId that is not yet present in the queue
        /// </summary>
        /// <param name="category"></param>
        /// <param name="items"></param>
        int Push(string category, IList<QueueItem> items);

        /// <summary>
        /// Gets the next n items in the queue of the give category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="numberToPop">max number of items that should be popped</param>
        /// <returns></returns>
        IList<QueueItem> Pop(string category, int numberToPop);

        int Count(string category);
        /// <summary>
        /// Delete all items related to the given category
        /// </summary>
        /// <param name="category"></param>
        void Delete(string category);
    }

    


}