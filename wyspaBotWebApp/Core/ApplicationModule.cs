using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using wyspaBotWebApp.Services;
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
            builder.RegisterType<RequestsService>().As<IRequestsService>();
            builder.RegisterType<WorldCupService>().As<IWorldCupService>();
            builder.RegisterType<WyspaBotService>().As<IWyspaBotService>()
                   .WithParameters(new List<Parameter> {
                       new NamedParameter("botName", this.botName),
                       new NamedParameter("channel", this.channelName)
                   }).SingleInstance();
            builder.RegisterType<PokemonApiService>().As<IPokemonApiService>();
            builder.RegisterType<PokemonService>().As<IPokemonService>();
            //builder.RegisterType<PasteBinApiService>().As<IPasteBinApiService>()
            //       .WithParameter(new NamedParameter("pastebinApiDevKey", this.pastebinApiDevKey));

            builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerRequest();
            //builder.RegisterAssemblyModules(typeof(MvcApplication).Assembly);
            builder.RegisterModule<AutofacWebTypesModule>();
        }
    }
}