using System;

namespace wyspaBotWebApp.Services.TasksManager {
    internal interface IWeeklyTask : ITask {
        DayOfWeek DayOfTheWeek { get; }

        DateTime TimeOfDay { get; }
    }
}