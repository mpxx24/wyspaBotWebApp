using System;
using System.Collections.Generic;
using System.Timers;
using NLog;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Services.Tasks;

namespace wyspaBotWebApp.Services.TasksManager {
    public class TaskService : ITaskService {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private int lastHour;

        private IEnumerable<Type> tasks;

        private Timer timer;

        public void WatchForTasks() {
            this.tasks = IoC.GetAllImplementationsOfInterface(typeof(ITask));

            this.timer = new Timer {Interval = 60000};
            this.timer.Elapsed += this.OnTimerClick;
            this.timer.Enabled = true;
        }

        private void OnTimerClick(object sender, ElapsedEventArgs e) {
            try {
                var dateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ApplicationSettingsHelper.TimeZoneInfo);

                foreach (var task in this.tasks) {
                    var hourlyTask = Activator.CreateInstance(task) as IHourlyTask;
                    if (hourlyTask != null && (dateTimeNow.Hour > this.lastHour || this.lastHour == 23 && dateTimeNow.Hour == 0)) {
                        this.lastHour = dateTimeNow.Hour;
                        hourlyTask.Run();
                        continue;
                    }

                    var dayilyTask = Activator.CreateInstance(task) as IDailyTask;
                    if (dayilyTask != null && (dateTimeNow.Hour == dayilyTask.TimeOfDay.Hour && dateTimeNow.Minute == dayilyTask.TimeOfDay.Minute)) {
                        dayilyTask.Run();
                        continue;
                    }

                    var weeklyTask = Activator.CreateInstance(task) as IWeeklyTask;
                    if (weeklyTask != null && (dateTimeNow.DayOfWeek == weeklyTask.DayOfTheWeek && dateTimeNow.Hour == weeklyTask.TimeOfDay.Hour && dateTimeNow.Minute == weeklyTask.TimeOfDay.Minute)) {
                        weeklyTask.Run();
                        continue;
                    }
                }
            }
            catch (Exception exception) {
                this.logger.Debug($"Exception occured while trying to run a task! {exception}");
            }
        }
    }
}