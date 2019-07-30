using System.Collections.Generic;
using System.Linq;

namespace wyspaBotWebApp.Core.Commands {
    public class IntroduceYourself : BaseCommand {
        public IntroduceYourself() {
            Aliases = new List<string> {string.Empty};
            Code = (splitInput, botName, postedMessages, chatUsers) => GetMessageToDisplay(CommandType.IntroduceYourselfCommand, botName);
        }
    }

    public class SayHelloToEveryoneInChat : BaseCommand {
        public SayHelloToEveryoneInChat() {
            Aliases = new List<string> {"hello", "elo", "hi", "siema", "yo", "siemanuby"};
            Code = (splitInput, botName, postedMessages, chatUsers) => {
                var userNick = this.GetUserNick(splitInput.ToList());
                return GetMessageToDisplay(CommandType.SayHelloToAllInTheChat, chatUsers.Where(x => !x.StartsWith("@") && x != userNick));
            };
        }
    }
}