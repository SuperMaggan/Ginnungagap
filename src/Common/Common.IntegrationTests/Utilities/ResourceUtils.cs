using System.IO;
using System.Linq;
using System.Reflection;

namespace Bifrost.Common.IntegrationTests.Utilities
{
    public static class ResourceUtils
    {
        public static Stream GetManifestResourceStream(string name)
        {
            var assembly = Assembly.Load(new AssemblyName("Bifrost.Common.IntegrationTests"));
            var resourceName = $"Bifrost.Common.IntegrationTests.{name}";

            var manifestResourceNames = assembly.GetManifestResourceNames().ToList();
            var exists = manifestResourceNames.Single(x => x == resourceName);
            return assembly.GetManifestResourceStream(resourceName);
        }

        public static string GetManifestString(string name)
        {
            var assembly = Assembly.Load(new AssemblyName("Bifrost.Common.IntegrationTests"));
            var resourceName = $"Bifrost.Common.IntegrationTests.{name}";

            var manifestResourceNames = assembly.GetManifestResourceNames().ToList();
            var exists = manifestResourceNames.Single(x => x == resourceName);
            using (var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
            {
                return reader.ReadToEnd();
            }

        }
    }
}
