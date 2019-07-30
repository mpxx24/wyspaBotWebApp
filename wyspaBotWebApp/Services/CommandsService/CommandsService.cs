using System;
using System.Collections.Generic;
using System.Linq;
using wyspaBotWebApp.Core;

namespace wyspaBotWebApp.Services.CommandsService {
    public class CommandsService : ICommandsService {
        public IEnumerable<BaseCommand> GetAllAvailableCommands() {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(x => x.IsSubclassOf(typeof(BaseCommand)) && !x.IsAbstract)
                .Select(x => (BaseCommand) Activator.CreateInstance(x));

            return types;
        }
    }
}