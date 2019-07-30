using System.Collections.Generic;

namespace wyspaBotWebApp.Core.Commands {
    public class WolframAlpha : BaseCommand {
        public WolframAlpha() {
            Aliases = new List<string> {"askq"};
            Code = (splitInput, botName, postedMessages, chatUsers) => {
                var question = GetPhraseWithoutCommandAndBotName(string.Join(" ", splitInput), "askq", botName);
                return GetMessageToDisplay(CommandType.WolframAlphaShortQuestionCommand, question);
            };
        }
    }
}