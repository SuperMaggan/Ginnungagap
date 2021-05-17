using Bifrost.Common.Core.Domain.DocumentProcessing;

namespace Bifrost.Common.Core.ApplicationServices
{
    /// <summary>
    /// Service used by the Document processing scripts
    /// Enables the scripts to easily manipulate data already stored in Hades
    /// </summary>
    public interface IProcessingUtilService
    {
        /// <summary>
        /// Saves the given utility object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="utilObject"></param>
        void SaveUtilObject<T>(T utilObject)
            where T : IProcessingUtilObject;

        /// <summary>
        /// Loads the utility object with the given key
        /// If not existing, null will be returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetUtilObject<T>(string key)
            where T : IProcessingUtilObject, new();

        /// <summary>
        /// Deletes the util object with the given key
        /// </summary>
        /// <param name="key"></param>
        void DeleteUtilObject(string key);

        TCustomUtilService GetCustomUtilService<TCustomUtilService>()
            where TCustomUtilService: class;
    }
}