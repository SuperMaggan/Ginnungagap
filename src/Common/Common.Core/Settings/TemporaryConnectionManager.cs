using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Bifrost.Common.Core.Settings
{
    /// <summary>
    /// Since ConnectionManager does not exist in netstandard 1.6 (due for 2.0) this is a temporary impl
    /// </summary>
    public static class TemporaryConnectionManager
    {
        
        public static string GetAppSetting(string key)
        {
            var xAppSettings = GetApplicationConfig().Element("appSettings");
            var xSetting = xAppSettings
                ?.Elements()
                .FirstOrDefault(x => x.Attribute("key").Value.Equals(key, StringComparison.OrdinalIgnoreCase));
            return xSetting?.Attribute("value")?.Value;

        }

        public static ConnectionStringSettings GetConnectionString(string name)
        {
            var xConnectionStrings = GetApplicationConfig().Element("connectionStrings");
            var xSetting = xConnectionStrings
                ?.Elements()
                .FirstOrDefault(x => x.Attribute("name").Value.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (xSetting == null)
                return null;
            var connectionString =  xSetting?.Attribute("connectionString")?.Value;
            var provider = xSetting?.Attribute("providerName")?.Value;
            return new ConnectionStringSettings()
            {
                ConnectionString = connectionString,
                ProviderName = provider


            }; 
        }


        public static XElement GetApplicationConfig()
        {
            var a = Assembly.GetEntryAssembly();
            var configFilePaths = Directory.EnumerateFiles(Directory.GetCurrentDirectory())
                .Where(x=>x.EndsWith(".config"));
            if(configFilePaths.Any())
                return XElement.Load(configFilePaths.First());
            throw new FileNotFoundException($"No config found was found. Places searched:\r\n\t{Directory.GetCurrentDirectory()} *.config");
        }
    }
}