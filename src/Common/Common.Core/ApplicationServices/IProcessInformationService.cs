using System.Collections.Generic;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.Core.ApplicationServices
{
    public interface IProcessInformationService
    {
        /// <summary>
        ///     Get this current process' information snapshot (not from the persistence store)
        /// </summary>
        /// <param name="processType">eg. JobsRunner, SearchService etc</param>
        /// <returns></returns>
        ProcessInformation CreateSnapshot(string processType);

        /// <summary>
        ///     Retrieves all persisted process informations
        /// </summary>
        /// <returns></returns>
        IList<ProcessInformation> Get();

        ProcessInformation Get(string id);

        /// <summary>
        ///     Presists the current process information
        ///     This is the same as Save( CreateSnapshot(string processType) )
        /// </summary>
        void SaveCurrent(string processType);

        /// <summary>
        ///     Persists the given processInformation
        /// </summary>
        /// <param name="processInformation"></param>
        void Save(ProcessInformation processInformation);

        /// <summary>
        ///     Delete all persisted Process information
        /// </summary>
        void Delete();

        /// <summary>
        ///     Delete the information with the given Id
        /// </summary>
        /// <param name="id"></param>
        void Delete(string id);
    }
}