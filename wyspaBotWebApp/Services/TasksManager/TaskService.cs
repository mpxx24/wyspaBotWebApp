using System;
using System.Collections.Generic;
using System.Timers;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Services.Tasks;

namespace wyspaBotWebApp.Services.TasksManager {
    public class TaskService : ITaskService {
        private IEnumerable<Type> tasks;
        private Timer timer;
        private int lastHour;

        public void WatchForTasks() {
            this.tasks = IoC.GetAllImplementationsOfInterface(typeof(ITask));

            this.timer = new Timer {Interval = 60000};
            this.timer.Elapsed += this.OnTimerClick;
            this.timer.Enabled = true;
        }

        private void OnTimerClick(object sender, ElapsedEventArgs e) {
            foreach (var task in this.tasks) {
                var instance = Activator.CreateInstance(task) as IHourlyTask;
                if (DateTime.Now.Hour > this.lastHour || this.lastHour == 23 && DateTime.Now.Hour == 0) {
                    this.lastHour = DateTime.Now.Hour;
                    instance?.Run();
                }
            }
        }
    }
}