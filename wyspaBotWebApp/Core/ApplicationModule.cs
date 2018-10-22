using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using NHibernate;
using wyspaBotWebApp.Services;
using wyspaBotWebApp.Services.GoogleMaps;
using wyspaBotWebApp.Services.Pokemon;

namespace wyspaBotWebApp.Core {
    public class ApplicationModule : Module {
        private readonly string botName;
        private readonly string channelName;
        private readonly string pastebinApiDevKey;

        public ApplicationModule(string pastebinApiDevKey, string channelName, string botName) {
            this.pastebinApiDevKey = pastebinApiDevKey;
            this.channelName = channelName;
            this.botName = botName;
        }

        protected override void Load(ContainerBuilder builder) {
            builder.Register(x => NhibernateHelper.OpenSession()).As<ISession>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            builder.RegisterType<RequestsService>().As<IRequestsService>();
            builder.RegisterType<WorldCupService>().As<IWorldCupService>();
            builder.RegisterType<WyspaBotService>().As<IWyspaBotService>()
                   .WithParameters(new List<Parameter> {
                       new NamedParameter("botName", this.botName),
                       new NamedParameter("channel", this.channelName)
                   }).SingleInstance();
            builder.RegisterType<PokemonApiService>().As<IPokemonApiService>();
            builder.RegisterType<PokemonService>().As<IPokemonService>();
            builder.RegisterType<GoogleMapsService>().As<IGoogleMapsService>();
            //builder.RegisterType<BotConfigurationService>().Named<IBotConfigurationService>("botConfigService");
            //builder.RegisterDecorator<IBotConfigurationService>((c, inner) => new BotConfigurationServiceDecorator(inner), "botConfigService");

            //builder.RegisterType<PasteBinApiService>().As<IPasteBinApiService>()
            //       .WithParameter(new NamedParameter("pastebinApiDevKey", this.pastebinApiDevKey));

            builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerRequest();
            //builder.RegisterAssemblyModules(typeof(MvcApplication).Assembly);
            builder.RegisterModule<AutofacWebTypesModule>();
        }
    }
}