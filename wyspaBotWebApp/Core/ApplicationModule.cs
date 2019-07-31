using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using NHibernate;
using wyspaBotWebApp.Services;
using wyspaBotWebApp.Services.Calendar;
using wyspaBotWebApp.Services.CommandsService;
using wyspaBotWebApp.Services.Email;
using wyspaBotWebApp.Services.GoogleMaps;
using wyspaBotWebApp.Services.Markov;
using wyspaBotWebApp.Services.NasaApi;
using wyspaBotWebApp.Services.PasteBin;
using wyspaBotWebApp.Services.Pokemon;
using wyspaBotWebApp.Services.Providers.Logs;
using wyspaBotWebApp.Services.Providers.PersonalData;
using wyspaBotWebApp.Services.Tasks;
using wyspaBotWebApp.Services.TasksManager;
using wyspaBotWebApp.Services.TasksManager.Tasks;
using wyspaBotWebApp.Services.WolframAlpha;
using wyspaBotWebApp.Services.WorldCup;
using wyspaBotWebApp.Services.Youtube;

namespace wyspaBotWebApp.Core {
    public class ApplicationModule : Module {
        private readonly string botName;
        private readonly string channelName;
        private readonly string nasaApiKey;
        private readonly string pastebinApiDevKey;
        private readonly string markovSourceFilePath;
        private readonly string youtubeApiKey;
        private readonly string wolframAlphaAppId;
        private readonly string mailSenderAddress;
        private readonly string mailSenderPassword;

        public ApplicationModule(string pastebinApiDevKey, string channelName, string botName, string nasaApiKey, string markovSourceFilePath, string youtubeApiKey, string wolframAlphaAppId, string mailSenderAddress, string mailSenderPassword) {
            this.pastebinApiDevKey = pastebinApiDevKey;
            this.channelName = channelName;
            this.botName = botName;
            this.nasaApiKey = nasaApiKey;
            this.markovSourceFilePath = markovSourceFilePath;
            this.youtubeApiKey = youtubeApiKey;
            this.wolframAlphaAppId = wolframAlphaAppId;
            this.mailSenderAddress = mailSenderAddress;
            this.mailSenderPassword = mailSenderPassword;
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
            builder.RegisterType<CalendarService>().As<ICalendarService>();
            builder.RegisterType<NasaApiService>().As<INasaApiService>()
                   .WithParameter(new NamedParameter("apiKey", this.nasaApiKey));

            builder.RegisterType<MarkovService>().As<IMarkovService>()
                   .WithParameter(new NamedParameter("markovSourceFilePath", this.markovSourceFilePath))
                   .WithParameter(new NamedParameter("level", 2))
                   .SingleInstance();

            builder.RegisterType<YoutubeService>().As<IYoutubeService>()
                   .WithParameter(new NamedParameter("youtubeApiKey", this.youtubeApiKey));

            builder.RegisterType<WolframAlphaService>().As<IWolframAlphaService>()
                   .WithParameter(new NamedParameter("wolframAlphaAppId", this.wolframAlphaAppId));

            builder.RegisterType<PasteBinApiService>().As<IPasteBinApiService>()
                   .WithParameter(new NamedParameter("pastebinApiDevKey", this.pastebinApiDevKey));

            builder.RegisterType<EmailService>().As<IEmailService>()
                   .WithParameter(new NamedParameter("mailSenderAddress", this.mailSenderAddress))
                   .WithParameter(new NamedParameter("mailSenderPassword", this.mailSenderPassword));

            builder.RegisterType<PersonalDataProvider>().As<IPersonalDataProvider>()
                .WithParameter(new NamedParameter("emailAddress", this.mailSenderAddress));

            builder.RegisterType<LogsProvider>().As<ILogsProvider>();

            builder.RegisterType<CommandsService>().As<ICommandsService>();

            builder.RegisterType<TaskService>().As<ITaskService>()
                   .SingleInstance();

            builder.RegisterType<MarkovPersistingTask>().As<ITask>();

            //dayily backup
            builder.RegisterType<DataBackupDailyTask>().As<ITask>();

            //weekly backup
            //builder.RegisterType<DataBackupWeeklyTask>().As<ITask>();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerRequest();
            builder.RegisterModule<AutofacWebTypesModule>();
        }
    }
}