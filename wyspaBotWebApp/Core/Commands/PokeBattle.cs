using System.Collections.Generic;
using System.Linq;

namespace wyspaBotWebApp.Core.Commands {
    public class PokeBattle : BaseCommand {
        public PokeBattle() {
            Aliases = new List<string> {"pokebattle"};
            Code = (splitInput, botName, postedMessages, chatUsers) => {
                if (splitInput.Count >= 6) {
                    var opponentName = splitInput[5];

                    if (!chatUsers.Contains(opponentName)) {
                        GetMessageToDisplay(CommandType.LogErrorCommand, $"User \"{opponentName}\" does not exist.");
                    }
                    else {
                        var nick = new List<string> {GetUserNick(splitInput.ToList()), opponentName};

                        GetMessageToDisplay(CommandType.PokeBattleCommand, nick);
                    }
                }
                else {
                    GetMessageToDisplay(CommandType.LogErrorCommand, "You need to specify opponent's name!)");
                }

                return new List<string>();
            };
        }
    }

    public class PokeBattleStats : BaseCommand {
        public PokeBattleStats() {
            Aliases = new List<string> {"pbstats"};
            Code = (splitInput, botName, postedMessages, chatUsers) => GetMessageToDisplay(CommandType.PokeBattleStatsCommand);
        }
    }

    public class ClearPokeBattleStats : BaseCommand {
        public ClearPokeBattleStats() {
            Aliases = new List<string> {"clpbs"};
            Code = (splitInput, botName, postedMessages, chatUsers) => GetMessageToDisplay(CommandType.ClearPokeBattleStatsCommand);
        }
    }
}