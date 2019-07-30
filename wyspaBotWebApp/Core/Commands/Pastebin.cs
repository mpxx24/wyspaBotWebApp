using System.Collections.Generic;

namespace wyspaBotWebApp.Core.Commands {
    public class History : BaseCommand {
        public History() {
            Aliases = new List<string> {"history"};
            Code = (splitInput, botName, postedMessages, chatUsers) => GetMessageToDisplay(CommandType.PasteToPastebinCommand, postedMessages);
        }
    }
}