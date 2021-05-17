using System.IO;
using System.Linq;
using System.Reflection;

namespace Bifrost.Common.ApplicationServices.Roslyn.Scripts
{
    public static class ResourceUtils
    {
        
        public static string GetManifestString(string name)
        {
            var assembly = Assembly.Load(new AssemblyName("Bifrost.Common.ApplicationServices.Roslyn"));
            var resourceName = $"Bifrost.Common.ApplicationServices.Roslyn.Scripts.{name}";

            var manifestResourceNames = assembly.GetManifestResourceNames().ToList();
            var exists = manifestResourceNames.Single(x => x == resourceName);
            using (var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
            {
                return reader.ReadToEnd();
            }

        }
    }
}