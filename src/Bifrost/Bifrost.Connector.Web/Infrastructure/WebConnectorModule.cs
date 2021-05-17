using Autofac;
using Bifrost.Connector.Web.ApplicationServices.Implementations;
using Bifrost.Connector.Web.ApplicationServices.Implementations.DownloadHandlers;
using Bifrost.Connector.Web.ApplicationServices.Implementations.LinkScrapers;
using Bifrost.Connector.Web.ApplicationServices.Implementations.PageConverters;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Core.Connectors;

namespace Bifrost.Connector.Web.Infrastructure
{
    public class WebConnectorModule : Module
    {

      protected override void Load(ContainerBuilder builder)
      {
        builder.RegisterType<WebCrawlerConnector>().As<IWebConnector>().SingleInstance();
        builder.RegisterType<ParallelPageService>().As<IPageService>().SingleInstance();
        builder.RegisterType<HttpClientDownloadHandler>().As<IDownloadHandler>().SingleInstance();
        builder.RegisterType<AgilityPackHrefLinkScraper>().As<ILinkScraper>().SingleInstance();
        builder.RegisterType<WebPageConverter>().As<IPageConverter>().SingleInstance();
        builder.RegisterType<BinaryPageConverter>().As<IPageConverter>().SingleInstance();

        builder.RegisterAssemblyTypes(System.Reflection.Assembly.GetAssembly(this.GetType()))
          .AssignableTo<IPageHandler>()
          .AsImplementedInterfaces()
          .SingleInstance();
      }




  }
}