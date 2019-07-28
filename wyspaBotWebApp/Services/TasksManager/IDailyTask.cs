using System;

namespace wyspaBotWebApp.Services.TasksManager {
    internal interface IDailyTask : ITask {
        //TODO: change to custom type
        DateTime TimeOfDay { get; }
    }
}