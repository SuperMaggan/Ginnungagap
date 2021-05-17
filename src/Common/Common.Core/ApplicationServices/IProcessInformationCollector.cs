using System.Collections.Generic;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.Core.ApplicationServices
{
    /// <summary>
    ///     Implementations will fetch component specific information
    /// </summary>
    public interface IProcessInformationCollector
    {
        /// <summary>
        ///     Fetch information that won't change. This will only be called during startup
        /// </summary>
        /// <returns></returns>
        IList<Field> GetStaleInformation();

        /// <summary>
        ///     Fetch information that changes during time. This will be called each time a new snapshot is created
        /// </summary>
        /// <returns></returns>
        IList<Field> GetSnapshotInformaiton();
    }
}