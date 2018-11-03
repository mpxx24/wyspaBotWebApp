using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using NLog;
using SpotifyApiWrapper.Core;
using wyspaBotWebApp.Core;

namespace wyspaBotWebApp {
    public class MvcApplication : HttpApplication {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var clientId = ConfigurationManager.AppSettings["clientId"];
            var secretId = ConfigurationManager.AppSettings["secretId"];
            var pastebinApiKey = ConfigurationManager.AppSettings["pastebinApiKey"];
            var botName = ConfigurationManager.AppSettings["botName"];
            var channelName = ConfigurationManager.AppSettings["channelName"];
            IoC.Initialize(new Module[] {new ApplicationModule(pastebinApiKey, channelName, botName)});
            SpotifyApiWrapperInitializer.Initialize(clientId, secretId);
        }

        protected void Application_Error() {
            var exc = this.Server.GetLastError();
            this.logger.Error(exc);
        }
    }
}