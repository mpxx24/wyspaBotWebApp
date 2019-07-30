using System.Collections.Generic;
using System.Linq;
using wyspaBotWebApp.Services.Markov;

namespace wyspaBotWebApp.Core.Commands {
    public class Markov : BaseCommand {
        public Markov() {
            Aliases = new List<string> {"markov"};
            Code = (splitInput, botName, postedMessages, chatUsers) => {
                var message = IoC.Resolve<IMarkovService>().GetText();
                return new List<string> {message};
            };
        }
    }

    public class MostUsedWords : BaseCommand {
        public MostUsedWords() {
            Aliases = new List<string> {"muw"};
            Code = (splitInput, botName, postedMessages, chatUsers) => IoC.Resolve<IMarkovService>().GetMostUsedWords().ToList();
        }
    }
}