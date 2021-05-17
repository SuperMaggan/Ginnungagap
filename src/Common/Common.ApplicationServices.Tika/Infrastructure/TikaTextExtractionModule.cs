using Autofac;
using Bifrost.Common.Core.ApplicationServices;

namespace Bifrost.Common.ApplicationServices.Tika.Infrastructure
{
  public class TikaTextExtractionModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<ParallellTextExtractionService>().As<ITextExtractionService>().SingleInstance();
      builder.RegisterType<TikaServerTextExtractor>().As<ITextExtractor>().SingleInstance();
      builder.RegisterType<TikaSettings>().AsSelf().SingleInstance();
    }
  }
}