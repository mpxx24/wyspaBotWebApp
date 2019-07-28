using System;
using System.IO;
using System.Text;
using NLog;
using wyspaBotWebApp.Core;

namespace wyspaBotWebApp.Services.Providers.Logs {
    public class LogsProvider : ILogsProvider {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public string GetCurrentLogs() {
            try {
                var file = File.ReadAllLines(ApplicationSettingsHelper.PathToLogFile);
                var sb = new StringBuilder();
                foreach (var line in file) {
                    sb.AppendLine(line);
                }
                return sb.ToString();
            }
            catch (Exception e) {
                this.logger.Error($"Failed while trying to read current log file! {e}");
                throw;
            }
        }
    }
}