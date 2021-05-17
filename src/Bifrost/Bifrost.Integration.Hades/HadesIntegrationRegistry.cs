using Asgard.Asgard.Core.ApplicationServices.Interfaces;
using Asgard.Hades.RestClient.Internal;
using StructureMap;

namespace Asgard.Integration.Hades
{
    public class HadesIntegrationRegistry : Registry
    {
        public HadesIntegrationRegistry()
        {

            ForSingletonOf<IHadesRestClientSettings>().Use<HadesRestClientSettings>();
            ForSingletonOf<IIntegrationService>().Use<HadesIntegrationService>();
        }
    }
}