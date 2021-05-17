using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bifrost.Common.Core.Domain.Jobs;
using Bifrost.Common.Core.Helpers;

namespace Bifrost.Common.Core.ApplicationServices.Helpers
{
    /// <summary>
    /// Default behaviour of serializing IJobConfiguration. 
    /// </summary>
    public class DefaultJobConfigurationMapper : IJobConfigurationMapper
    {
        private readonly IJobConfiguration[] _registeredConfigTypes;

        public DefaultJobConfigurationMapper(IJobConfiguration[] registeredConfigTypes)
        {
            _registeredConfigTypes = registeredConfigTypes;
        }

        public virtual string Serialize(IJobConfiguration config)
        {
            if (config == null)
                return "";
            return SerializeToXElement(config).ToString();
        }

        public virtual IJobConfiguration Deserialize(string jobConfiguration)
        {
            if (string.IsNullOrEmpty(jobConfiguration))
                return null;
            return Deserialize(XElement.Parse(jobConfiguration));
        }

        public virtual IJobConfiguration Deserialize(XElement jobConfiguration)
        {
            if (jobConfiguration == null)
                return null;
            var typeName = jobConfiguration.Name.LocalName;
            var configType = _registeredConfigTypes.FirstOrDefault(c => c.GetType().Name == typeName);
            if (configType == null)
                throw new Exception(
                    $"No IJobConfiguration of type {typeName} was registered. Please register it in your container");
            return jobConfiguration.FromXElement(configType.GetType()) as IJobConfiguration;
            
        }

        public IList<IJobConfiguration> GetAllAvailable()
        {
            return _registeredConfigTypes;
        }

        protected XElement SerializeToXElement(IJobConfiguration config)
        {
            return config?.ToXElement(config.GetType());
        }


    }
}