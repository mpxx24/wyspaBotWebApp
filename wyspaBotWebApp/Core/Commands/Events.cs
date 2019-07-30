using System.Collections.Generic;
using System.Linq;

namespace wyspaBotWebApp.Core.Commands {
    public class AddEvent : BaseCommand {
        public AddEvent() {
            Aliases = new List<string> { "addevent" };
            Code = (splitInput, botName, postedMessages, chatUsers) => {
                if (splitInput.Count >= 7) {
                    var whoAdded = this.GetUserNick(splitInput.ToList());
                    var realPhrase = this.GetPhraseWithoutCommandAndBotName(string.Join(" ", splitInput), "addevent", botName);

                    var realArguments = realPhrase.Split('-').Select(x => x.Trim()).ToList();
                    if (realArguments.Count != 3) {
                        return this.GetMessageToDisplay(CommandType.LogErrorCommand, "You need to pass exactly 3 parameters seperated by '-' sign!");
                    }

                    return this.GetMessageToDisplay(CommandType.AddEventCommand, new List<string> {whoAdded, realArguments[0], realArguments[1], realArguments[2]});
                }

                return this.GetMessageToDisplay(CommandType.LogErrorCommand, "You need to specify both: event name and time!)");
            };
        }
    }

    public class ListEvents : BaseCommand {
        public ListEvents() {
            Aliases = new List<string> { "listevents" };
            Code = (splitInput, botName, postedMessages, chatUsers) => this.GetMessageToDisplay(CommandType.ListAllEventsCommand);
        }
    }

    public class NextEvents : BaseCommand {
        public NextEvents() {
            Aliases = new List<string> {"nextevent"};
            Code = (splitInput, botName, postedMessages, chatUsers) => this.GetMessageToDisplay(CommandType.GetNextEventCommand);
        }
    }
}