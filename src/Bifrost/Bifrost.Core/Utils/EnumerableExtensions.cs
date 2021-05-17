using System.Collections.Generic;

namespace Bifrost.Core.Utils
{
    public static class EnumerableExtensions
    {

        /// <summary>
        /// Batches the elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="batchSizeLimit"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> InBatches<T>(this IEnumerable<T> collection, int batchSizeLimit)
        {
            var nextbatch = new List<T>(batchSizeLimit);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSizeLimit)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSizeLimit);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }



      

    }
}
