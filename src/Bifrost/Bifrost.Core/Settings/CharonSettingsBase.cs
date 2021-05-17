using System.Linq;
using Bifrost.Common.Core.Settings;
using Serilog;

namespace Bifrost.Core.Settings
{
    public abstract class AsgardSettingsBase : ConfigSettingsBase
    {
        protected AsgardSettingsBase()
        {
            var set = GetSetSettingsMessages().ToArray();
            if(set.Any())
            {
                Log.Information($"Found settings ({GetType().Name}):\r\n\t{string.Join("\r\n\t", set)}");
            }
                
            var missing = GetNotSetSettingsMessages().Where(msg => !msg.Contains("ILogger")).ToArray();
            if(missing.Any())
                Log.Information($"Missing settings ({GetType().Name}):\r\n\t{string.Join("\r\n\t", missing)}");
        }
    }
}