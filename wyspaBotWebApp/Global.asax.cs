using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using NLog;
using SpotifyApiWrapper.Core;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Services.Markov;

namespace wyspaBotWebApp {
    public class MvcApplication : HttpApplication {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start() {
            this.logger.Debug("Application start!");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var clientId = ConfigurationManager.AppSettings["clientId"];
            var secretId = ConfigurationManager.AppSettings["secretId"];
            var pastebinApiKey = ConfigurationManager.AppSettings["pastebinApiKey"];
            var botName = ConfigurationManager.AppSettings["botName"];
            var channelName = ConfigurationManager.AppSettings["channelName"];
            var nasaApiKey = ConfigurationManager.AppSettings["nasaApiKey"];
            var markovSourceFilePath = ConfigurationManager.AppSettings["markovSourceFilePath"];
            var youtubeApiKey = ConfigurationManager.AppSettings["youtubeApiKey"];
            var wolframAlphaAppId = ConfigurationManager.AppSettings["wolframAlphaAppId"];
            var mailSenderAddress = ConfigurationManager.AppSettings["mailSenderAddress"];
            var mailSenderPassword = ConfigurationManager.AppSettings["mailSenderPassword"];
            IoC.Initialize(new Module[] {new ApplicationModule(pastebinApiKey, channelName, botName, nasaApiKey, markovSourceFilePath, youtubeApiKey, wolframAlphaAppId, mailSenderAddress, mailSenderPassword) });
            SpotifyApiWrapperInitializer.Initialize(clientId, secretId);
        }

        protected void Application_Error() {
            this.logger.Debug("Application error!");
            IoC.Resolve<IMarkovService>().PersistMarkovObject();

            var exc = this.Server.GetLastError();
            this.logger.Error(exc);
        }

        public void Application_End() {
            this.logger.Debug("Application end!");
            IoC.Resolve<IMarkovService>().PersistMarkovObject();
        }

        protected void Session_End(object sender, EventArgs e) {
            //hacks!
            if (IoC.IsInitialized) {
                this.logger.Debug("Session end!");
                IoC.Resolve<IMarkovService>().PersistMarkovObject();
            }
        }
    }
}