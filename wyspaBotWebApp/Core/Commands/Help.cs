using System.Collections.Generic;

namespace wyspaBotWebApp.Core.Commands {
    public class Help : BaseCommand {
        public Help() {
            Aliases = new List<string> {"help", "h"};
            Code = (splitInput, botName, postedMessages, chatUsers) => GetMessageToDisplay(CommandType.HelpCommand);
        }
    }
}