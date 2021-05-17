using Autofac;
using Bifrost.Common.Core.ApplicationServices;

namespace Bifrost.Common.ApplicationServices.Roslyn.Infrastructure
{
    public class RoslynScriptProcessingModule : Module
    {
      protected override void Load(ContainerBuilder builder)
      {
          builder.RegisterType<IScriptProcessingService>().As<RoslynScriptProcessingService>();
          builder.RegisterType<ScriptProcessingSettings>().AsSelf();
      }
    }
}