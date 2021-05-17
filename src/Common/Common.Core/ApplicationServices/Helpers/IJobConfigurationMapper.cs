using System.Collections.Generic;
using System.Xml.Linq;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.Core.ApplicationServices.Helpers
{
    /// <summary>
    /// Provides an interface for serializing a IjobConfiguration 
    /// </summary>
    public interface IJobConfigurationMapper
    {
        string Serialize(IJobConfiguration config);



        IJobConfiguration Deserialize(string jobConfiguration);
        /// <summary>
        /// Deserialize the given configuration to a type decided by teh given xml's root element name
        /// </summary>
        /// <param name="jobConfiguration"></param>
        /// <returns></returns>
        IJobConfiguration Deserialize(XElement jobConfiguration);

        /// <summary>
        /// Returns all available configurations
        /// </summary>
        /// <returns></returns>
        IList<IJobConfiguration> GetAllAvailable();

    }
}