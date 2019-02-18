using System;
using NLog;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Services.Markov;

namespace wyspaBotWebApp.Services.TasksManager.Tasks {
    public class MarkovPersistingTask : IHourlyTask {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void Run() {
            try {
                this.logger.Debug($"Running task {nameof(MarkovPersistingTask)}.");
                IoC.Resolve<IMarkovService>().PersistMarkovObject();

                this.logger.Debug($"Task {nameof(MarkovPersistingTask)} finished.");
            }
            catch (Exception e) {
                this.logger.Debug($"Exception occured while trying to run the {nameof(MarkovPersistingTask)} task! {e}");
            }
        }
    }
}