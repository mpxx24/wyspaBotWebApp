using System;
using System.Collections.Generic;
using System.Linq;

namespace wyspaBotWebApp.Core {
    public abstract class Command {
        public IEnumerable<string> Aliases { get; set; }
        public CommandType CommandType { get; set; }
        public Func<IList<string>, IList<string>> Code { get; set; }

        private IList<string> GetMessageToDisplay() {
            var factory = new CommandFactory();
            return factory.GetCommand(CommandType).GetText().ToList();
        }

        public IList<string> GetMessageToDisplay(string parameter) {
            var factory = new CommandWithStringParameterFactory();
            return factory.GetCommand(CommandType).GetText(parameter).ToList();
        }

        public IList<string> GetMessageToDisplay(string parameter, int value) {
            var factory = new CommandWithTwoParametersFactory();
            return factory.GetCommand(CommandType).GetText(parameter, value).ToList();
        }

        public IList<string> GetMessageToDisplay(IEnumerable<string> parameter) {
            var factory = new CommandWithStringIenumerableParameterFactory();
            return factory.GetCommand(CommandType).GetText(parameter).ToList();
        }

        public IList<string> WyspaBotSayPrivate(string nick) {
            var factory = new CommandFactory();
            return factory.GetCommand(CommandType).GetText().ToList();
        }
    }

    public class ExampleCommand : Command {
        public ExampleCommand() {
            Aliases = new List<string> {"rtracks"};
            CommandType = CommandType.RecommendedTracksBasedOnTrackCommand;
            Code = splitInput => {
                if (splitInput.Count >= 6) {
                    var trackId = splitInput[5];
                    var limit = 10;

                    if (splitInput.Count >= 7) {
                        var limitAsString = splitInput[6];
                        int.TryParse(limitAsString, out limit);
                    }

                    return GetMessageToDisplay(trackId, limit);
                }
                return new List<string>();
            };
        }
    }
}