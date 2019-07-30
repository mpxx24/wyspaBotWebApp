using System.Collections.Generic;
using wyspaBotWebApp.Core;

namespace wyspaBotWebApp.Services.CommandsService {
    public interface ICommandsService {
        IEnumerable<BaseCommand> GetAllAvailableCommands();
    }
}