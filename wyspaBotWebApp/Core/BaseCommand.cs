using System;
using System.Collections.Generic;
using System.Linq;

namespace wyspaBotWebApp.Core {
    public abstract class BaseCommand {
        public IEnumerable<string> Aliases { get; set; }
        public Func<IList<string>, string, IList<string>, IList<string>, IList<string>> Code { get; set; }

        protected IList<string> GetMessageToDisplay(CommandType commandType) {
            var factory = new CommandFactory();
            return factory.GetCommand(commandType).GetText().ToList();
        }

        protected IList<string> GetMessageToDisplay(CommandType commandType, string parameter) {
            var factory = new CommandWithStringParameterFactory();
            return factory.GetCommand(commandType).GetText(parameter).ToList();
        }

        protected IList<string> GetMessageToDisplay(CommandType commandType, string parameter, int value) {
            var factory = new CommandWithTwoParametersFactory();
            return factory.GetCommand(commandType).GetText(parameter, value).ToList();
        }

        protected IList<string> GetMessageToDisplay(CommandType commandType, IEnumerable<string> parameter) {
            var factory = new CommandWithStringIenumerableParameterFactory();
            return factory.GetCommand(commandType).GetText(parameter).ToList();
        }

        protected string GetUserNick(IReadOnlyList<string> splitInput) {
            var indexOfExclamationMark = splitInput[0].IndexOf('!');
            return indexOfExclamationMark == -1
                ? string.Empty
                : splitInput[0].Substring(1, indexOfExclamationMark - 1);
        }

        protected string GetPhraseWithoutCommandAndBotName(string phrase, string command, string botName) {
            var commandWithDash = $"-{command}";
            var fullBotName = $"{botName}:";

            var indexOfFullBotName = phrase.IndexOf(fullBotName, StringComparison.InvariantCulture);

            var isBotNameUsedWithColon = indexOfFullBotName != -1;
            if (!isBotNameUsedWithColon) {
                indexOfFullBotName = phrase.IndexOf(botName, StringComparison.InvariantCulture);
            }

            phrase = phrase.Remove(indexOfFullBotName, isBotNameUsedWithColon ? fullBotName.Length : botName.Length);

            var indexOfCommandWithDash = phrase.IndexOf(commandWithDash, StringComparison.InvariantCulture);
            var indexOfCommand = phrase.IndexOf(command, StringComparison.InvariantCulture);

            var doesPhraseContainCommandWithDash = indexOfCommandWithDash != -1;
            var doesPhraseContainCommand = indexOfCommand != -1;

            return doesPhraseContainCommandWithDash
                ? phrase.Remove(indexOfCommandWithDash, commandWithDash.Length)
                : doesPhraseContainCommand
                    ? phrase.Remove(indexOfCommand, command.Length)
                    : phrase;
        }
    }
}