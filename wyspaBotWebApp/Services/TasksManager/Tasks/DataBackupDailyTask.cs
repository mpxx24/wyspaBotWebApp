using System;
using System.Collections.Generic;
using NLog;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Services.Email;
using wyspaBotWebApp.Services.Markov;
using wyspaBotWebApp.Services.Providers.PersonalData;

namespace wyspaBotWebApp.Services.TasksManager.Tasks {
    public class DataBackupDailyTask : IDailyTask {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public DateTime TimeOfDay => new DateTime(DateTime.Now.Year, 1, 1, 20, 30, 0);

        public void Run() {
            try {
                this.logger.Debug($"Running task {nameof(DataBackupDailyTask)}.");

                var recipient = IoC.Resolve<IPersonalDataProvider>().GetEmailForBackups();

                IoC.Resolve<IEmailService>().SendEmailWithAttachments(new List<string> {recipient},
                                                                      "Backup!",
                                                                      "DB file and markov source",
                                                                      new List<string> {
                                                                          ApplicationSettingsHelper.PathToDbFile,
                                                                          IoC.Resolve<IMarkovService>().GetPathToSourceFile()
                                                                      });

                this.logger.Debug($"Task {nameof(DataBackupDailyTask)} finished.");
            }
            catch (Exception e) {
                this.logger.Debug($"Exception occured while trying to run the {nameof(DataBackupDailyTask)} task! {e}");
            }
        }
    }
}