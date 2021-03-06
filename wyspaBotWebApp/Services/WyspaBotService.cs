﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using NLog;
using wyspaBotWebApp.Common;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Services.Markov;
using wyspaBotWebApp.Services.Tasks;
using wyspaBotWebApp.Services.Youtube;

namespace wyspaBotWebApp.Services {
    public class WyspaBotService : IWyspaBotService {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private StreamWriter writer;
        private StreamReader reader;
        private readonly string messageAlias = "PRIVMSG";
        private readonly int port = 6667;
        private readonly ListWithSpecifiedSize<string> postedMessages = new ListWithSpecifiedSize<string>(200);
        private readonly List<string> chatUsers = new List<string>();
        private readonly List<string> textFaces = new List<string> {"( ͡° ͜ʖ ͡°)", "(ง ͠° ͟ل͜ ͡°)ง", "(ง'̀-'́)ง", "☜(ﾟヮﾟ☜)", "~(˘▾˘~)", "༼ つ  ͡° ͜ʖ ͡° ༽つ", "(ง°ل͜°)ง"};
        private readonly string throwingTableString = ":(╯°□°）╯︵┻━┻";
        private readonly string user;
        private TcpClient irc;
        private readonly string server = "irc.freenode.net";
        private readonly List<string> lastInnerException = new List<string>();
        private bool shouldStartSavingMessages;
        private string startReadingWhenContainsString = "Thank you for using freenode";

        private readonly string botName;
        private readonly string channel;
        private readonly IMarkovService markovService;
        private readonly IYoutubeService youtubeService;
        private readonly ITaskService taskService;

        public WyspaBotService(string channel, string botName, IMarkovService markovService, IYoutubeService youtubeService, ITaskService taskService) {
            this.channel = channel;
            this.botName = botName;
            this.markovService = markovService;
            this.youtubeService = youtubeService;
            this.taskService = taskService;
            this.user = $"USER {this.botName} 0 * :{this.botName}";
        }

        public void StartBot() {
            try {
                this.logger.Debug($"Starting {this.botName} - connecting to: {this.server}:{this.port}");
                this.irc = new TcpClient(this.server, this.port);

                var stream = this.irc.GetStream();
                this.reader = new StreamReader(stream);
                this.writer = new StreamWriter(stream);
                this.writer.WriteLine("NICK " + this.botName);
                this.writer.Flush();
                this.writer.WriteLine(this.user);
                this.writer.Flush();

                this.taskService.WatchForTasks();
                this.ReadChat();
            }
            catch (Exception e) {
                this.logger.Debug($"Exception occured! {e}");

                this.SaveLastInnerException(e);
            }
        }

        public void StopBot() {
            this.logger.Debug($"Stopping {this.botName}!");
            this.markovService.PersistMarkovObject();
            this.chatUsers.Clear();
            this.irc.Close();
        }

        private void ReadChat() {
            this.logger.Debug($"Starting to read the chat! Channel {this.channel}");
            while (true) {
                string inputLine;
                while ((inputLine = this.reader.ReadLine()) != null) {
                    try {
                        var splitInput = inputLine.Split(' ').ToList();
                        var phrase = this.GetPhrase(inputLine);

                        if (splitInput[0] == "PING") {
                            var pongReply = splitInput[1];
                            this.SendData("PONG " + pongReply);
                        }

                        if (splitInput[1] == "001") {
                            var joinString = "JOIN " + this.channel;
                            this.SendData(joinString);
                        }

                        if (splitInput[1] == "NICK") {
                            var previousNick = this.GetUserNick(splitInput);
                            var newNick = splitInput[2].Substring(1, splitInput[2].Length - 1);
                            if (this.chatUsers.Any(x => x == previousNick)) {
                                var indexOf = this.chatUsers.IndexOf(previousNick);
                                this.chatUsers[indexOf] = newNick;
                            }
                            else if (this.chatUsers.Any(x => x == $"@{previousNick}")) {
                                var indexOf = this.chatUsers.IndexOf($"@{previousNick}");
                                this.chatUsers[indexOf] = newNick;
                            }
                            else {
                                this.chatUsers.Add(newNick);
                            }
                        }

                        if (splitInput.Count >= 6 && splitInput[2] == this.botName && (splitInput[3] == "@" || splitInput[3] == "=") && splitInput[4] == this.channel) {
                            this.chatUsers.Clear();
                            var numberOfPeopleInChat = splitInput.Count - 6;

                            for (var i = 0; i < numberOfPeopleInChat; i++) {
                                this.chatUsers.Add(splitInput[6 + i]);
                            }

                            this.WyspaBotSay(CommandType.SayHelloAfterJoining, this.chatUsers);
                        } else if (splitInput.Count >= 6 && splitInput[4].ToLowerInvariant() == this.channel.ToLowerInvariant() && splitInput[5].Contains(this.botName)) {
                            this.chatUsers.Clear();
                            var numberOfPeopleInChat = splitInput.Count - 6;

                            for (var i = 0; i < numberOfPeopleInChat; i++) {
                                var nick = splitInput[6 + i];
                                if (!nick.StartsWith("@") && !nick.StartsWith(":")) {
                                    this.chatUsers.Add(nick);
                                }
                            }

                            this.WyspaBotSay(CommandType.SayHelloAfterJoining, this.chatUsers);
                        }

                        if (splitInput[1] == "JOIN") {
                            var nick = this.GetUserNick(splitInput);
                            if (nick != this.botName) {
                                if (this.chatUsers.All(x => x != nick)) {
                                    this.chatUsers.Add(nick);
                                }
                                var random = new Random();

                                var parameters = new List<string> {nick, this.channel, this.textFaces[random.Next(this.textFaces.Count)]};
                                this.WyspaBotSay(CommandType.SayHelloToNewcomerCommand, parameters);
                            }
                        }

                        if (splitInput[1] == "QUIT") {
                            var nick = this.GetUserNick(splitInput);
                            if (nick != this.botName) {
                                if (this.chatUsers.Any(x => x == nick)) {
                                    this.chatUsers.RemoveAll(x => x == nick);
                                }
                            }
                        }

                        if (splitInput.Count >= 3 && splitInput[1] == this.messageAlias && splitInput[2] == this.botName) {
                            var nick = this.GetUserNick(splitInput);
                            this.WyspaBotSayPrivate(CommandType.StopUsingPrivateChannelCommand, nick);
                        }

                        if (phrase.Contains(this.startReadingWhenContainsString)) {
                            this.shouldStartSavingMessages = true;
                        }

                        if (splitInput.Count >= 4 && this.shouldStartSavingMessages && !phrase.StartsWith(this.botName) && phrase != this.startReadingWhenContainsString) {
                            var nick = this.GetUserNick(splitInput);

                            if (!string.IsNullOrEmpty(nick) && !nick.Contains('_')) {
                                this.postedMessages.Add($"<{nick}> {phrase}");
                                this.markovService.Learn(phrase);
                            }
                        }

                        if (splitInput.Count >= 4 && this.youtubeService.IsYoutubeVideoLink(phrase)) {
                            this.WyspaBotSay(CommandType.GetYoutubeVideoTitleCommand, phrase);
                        }
                        else if (splitInput.Count >= 4 && splitInput.Any(x => this.youtubeService.IsYoutubeVideoLink(x))) {
                            var link = splitInput.FirstOrDefault(x => this.youtubeService.IsYoutubeVideoLink(x));
                            if (!string.IsNullOrEmpty(link)) {
                                this.WyspaBotSay(CommandType.GetYoutubeVideoTitleCommand, link);
                            }
                        }

                        if (splitInput.Count >= 4 && splitInput[3].Trim() == this.throwingTableString || splitInput.Count >= 5 && (splitInput[3] + splitInput[4]).Trim() == this.throwingTableString) {
                            this.WyspaBotSay(CommandType.PutTheTableBackCommand);
                        }

                        if (splitInput.Count >= 4 && (splitInput[3] == $":{this.botName}" || splitInput[3] == $":{this.botName}:")) {
                            if (splitInput.Count == 4) {
                                splitInput.Add(string.Empty);
                            }

                            switch (splitInput[4].Trim().ToLowerInvariant()) {
                                case "":
                                    this.WyspaBotSay(CommandType.IntroduceYourselfCommand, this.botName);
                                    break;
                                case "help":
                                case "h":
                                    this.WyspaBotSay(CommandType.HelpCommand);
                                    break;
                                case "rtracks":
                                    if (splitInput.Count >= 6) {
                                        var trackId = splitInput[5];
                                        var limit = 10;

                                        if (splitInput.Count >= 7) {
                                            var limitAsString = splitInput[6];
                                            int.TryParse(limitAsString, out limit);
                                        }

                                        this.WyspaBotSay(CommandType.RecommendedTracksBasedOnTrackCommand, trackId, limit);
                                    }
                                    break;
                                //case "-wct":
                                //case "wct":
                                //    this.WyspaBotSay(CommandType.TodaysWorldCupGamesCommand);
                                //    break;
                                //case "-wcy":
                                //case "wcy":
                                //    this.WyspaBotSay(CommandType.YesterdaysWorldCupGamesAndScoresCommand);
                                //    break;
                                case "history":
                                    this.WyspaBotSay(CommandType.PasteToPastebinCommand, this.postedMessages);
                                    break;
                                case "-cltmp":
                                    this.postedMessages.Clear();
                                    break;
                                case "pokebattle":
                                    if (splitInput.Count >= 6) {
                                        var opponentName = splitInput[5];

                                        if (!this.chatUsers.Contains(opponentName)) {
                                            this.WyspaBotSay(CommandType.LogErrorCommand, $"User \"{opponentName}\" does not exist.");
                                        }
                                        else {
                                            var nick = new List<string> {this.GetUserNick(splitInput), opponentName};

                                            this.WyspaBotSay(CommandType.PokeBattleCommand, nick);
                                        }
                                    }
                                    else {
                                        this.WyspaBotSay(CommandType.LogErrorCommand, "You need to specify opponent's name!)");
                                    }
                                    break;
                                case "ghrepo":
                                    this.WyspaBotSay(CommandType.GetRepositoryAddressCommand);
                                    break;
                                case "pbstats":
                                    this.WyspaBotSay(CommandType.PokeBattleStatsCommand);
                                    break;
                                case "clpbs":
                                    this.WyspaBotSay(CommandType.ClearPokeBattleStatsCommand);
                                    break;
                                case "gmdistance":
                                    if (splitInput.Count >= 7) {
                                        var origin = splitInput[5];
                                        var destination = splitInput[6];

                                        this.WyspaBotSay(CommandType.GoogleMapDistanceCommand, new List<string> {origin, destination});
                                    }
                                    else {
                                        this.WyspaBotSay(CommandType.LogErrorCommand, "You need to specify both: origin and destination!)");
                                    }
                                    break;
                                case "markov":
                                    var message = this.markovService.GetText();
                                    this.WyspaBotDebug(new List<string> {message});
                                    break;
                                case "addevent":
                                    if (splitInput.Count >= 7) {
                                        var whoAdded = this.GetUserNick(splitInput);
                                        var realPrhase = this.GetPhraseWithoutCommandAndBotName(phrase, "addevent");

                                        var realArguments = realPrhase.Split('-').Select(x => x.Trim()).ToList();
                                        if (realArguments.Count != 3) {
                                            this.WyspaBotSay(CommandType.LogErrorCommand, "You need to pass exactly 3 parameters seperated by '-' sign!");
                                            break;
                                        }

                                        this.WyspaBotSay(CommandType.AddEventCommand, new List<string> {whoAdded, realArguments[0], realArguments[1], realArguments[2]});
                                    }
                                    else {
                                        this.WyspaBotSay(CommandType.LogErrorCommand, "You need to specify both: event name and time!)");
                                    }
                                    break;
                                case "listevents":
                                    this.WyspaBotSay(CommandType.ListAllEventsCommand);
                                    break;
                                case "nextevent":
                                    this.WyspaBotSay(CommandType.GetNextEventCommand);
                                    break;
                                case "npod":
                                    this.WyspaBotSay(CommandType.NasaPictureOfTheDayCommand);
                                    break;
                                case "-debug":
                                    this.WyspaBotDebug(this.lastInnerException);
                                    break;
                                case "hello":
                                case "elo":
                                case "hi":
                                case "yo":
                                case "siema":
                                    var userNick = this.GetUserNick(splitInput);
                                    this.WyspaBotSay(CommandType.SayHelloToAllInTheChat, this.chatUsers.Where(x => !x.StartsWith("@") && x != userNick));
                                    break;
                                case "askq":
                                    var question = this.GetPhraseWithoutCommandAndBotName(phrase, "askq");
                                    this.WyspaBotSay(CommandType.WolframAlphaShortQuestionCommand, question);
                                    break;
                                case "-rmevents":
                                    this.WyspaBotSay(CommandType.ResetAllEventsCommand);
                                    break;
                                case "muw":
                                    var wordUseStats = this.markovService.GetMostUsedWords();
                                    this.WyspaBotSay(CommandType.GetMostUsedWordsCommand, wordUseStats);
                                    break;
                                default:
                                    if (splitInput.Any(x => x.Contains(this.botName))) {
                                        this.WyspaBotSay(CommandType.ResponseWhenMentionedCommand);
                                    }
                                    break;
                            }
                        }
                    }
                    catch (Exception e) {
                        this.logger.Debug($"Failed to generate command response! Text {inputLine}. {e}");

                        this.SaveLastInnerException(e);
                    }
                }
            }
        }

        private string GetUserNick(IReadOnlyList<string> splitInput) {
            var indexOfExclamationMark = splitInput[0].IndexOf('!');
            return indexOfExclamationMark == -1
                ? string.Empty
                : splitInput[0].Substring(1, indexOfExclamationMark - 1);
        }

        public void SendData(string cmd, string param = "") {
            if (param == null) {
                this.writer.WriteLine(cmd);
                this.writer.Flush();
            }
            else {
                this.writer.WriteLine(cmd + " " + param);
                this.writer.Flush();
            }
        }

        private void WyspaBotSay(CommandType commandType) {
            var factory = new CommandFactory();
            var list = factory.GetCommand(commandType).GetText();

            foreach (var message in list) {
                this.SendData($"{this.messageAlias} {this.channel} :{message}");
            }
        }

        private void WyspaBotSay(CommandType commandType, string parameter) {
            var factory = new CommandWithStringParameterFactory();
            var list = factory.GetCommand(commandType).GetText(parameter);

            foreach (var message in list) {
                this.SendData($"{this.messageAlias} {this.channel} :{message}");
            }
        }

        private void WyspaBotSay(CommandType commandType, string parameter, int value) {
            var factory = new CommandWithTwoParametersFactory();
            var list = factory.GetCommand(commandType).GetText(parameter, value);

            foreach (var message in list) {
                this.SendData($"{this.messageAlias} {this.channel} :{message}");
            }
        }

        private void WyspaBotSay(CommandType commandType, IEnumerable<string> parameter) {
            var factory = new CommandWithStringIenumerableParameterFactory();
            var list = factory.GetCommand(commandType).GetText(parameter);

            foreach (var message in list) {
                this.SendData($"{this.messageAlias} {this.channel} :{message}");
                Thread.Sleep(750);
            }
        }

        private void WyspaBotSayPrivate(CommandType commandType, string nick) {
            var factory = new CommandFactory();
            var list = factory.GetCommand(commandType).GetText();

            foreach (var message in list) {
                this.SendData($"{this.messageAlias} {nick} :{message}");
            }
        }

        private void WyspaBotDebug(IEnumerable<string> list) {
            foreach (var message in list) {
                this.SendData($"{this.messageAlias} {this.channel} :{message}");
                Thread.Sleep(750);
            }
        }

        private string GetPhraseWithoutCommandAndBotName(string phrase, string command) {
            var commandWithDash = $"-{command}";
            var fullBotName = $"{this.botName}:";

            var indexOfFullBotName = phrase.IndexOf(fullBotName, StringComparison.InvariantCulture);

            var isBotNameUsedWithColon = indexOfFullBotName != -1;
            if (!isBotNameUsedWithColon) {
                indexOfFullBotName = phrase.IndexOf(this.botName, StringComparison.InvariantCulture);
            }
            phrase = phrase.Remove(indexOfFullBotName, isBotNameUsedWithColon ? fullBotName.Length : this.botName.Length);

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

        private void SaveLastInnerException(Exception e) {
            this.WyspaBotSay(CommandType.LogErrorCommand, "Something went wrong (>ლ)");
            this.lastInnerException.Clear();

            var excSplit = e.InnerException?.ToString().Split(new[] {"\r\n"}, StringSplitOptions.None);
            if (excSplit != null) {
                foreach (var m in excSplit) {
                    if (m.Contains("End of inner exception stack trace")) {
                        break;
                    }
                    this.lastInnerException.Add(m);
                }
            }
        }

        private string GetPhrase(string inputLine)
        {
            var separator = $"{this.channel.ToLowerInvariant()} :";
            return inputLine.Substring(inputLine.IndexOf(separator, StringComparison.Ordinal) + separator.Length);
        }
    }
}