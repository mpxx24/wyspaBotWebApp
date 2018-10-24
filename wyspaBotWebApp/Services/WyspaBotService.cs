using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using MarkovSharp.TokenisationStrategies;
using wyspaBotWebApp.Core;

namespace wyspaBotWebApp.Services {
    public class WyspaBotService : IWyspaBotService {
        public readonly string botName;
        public readonly string channel;

        private static StreamWriter writer;
        private readonly string messageAlias = "PRIVMSG";
        private readonly int port = 6667;
        private readonly List<string> postedMessages = new List<string>();
        private readonly List<string> chatUsers = new List<string>();
        private readonly List<string> textFaces = new List<string> { "( ͡° ͜ʖ ͡°)", "(ง ͠° ͟ل͜ ͡°)ง", "(ง'̀-'́)ง", "☜(ﾟヮﾟ☜)", "~(˘▾˘~)", "༼ つ  ͡° ͜ʖ ͡° ༽つ", "(ง°ل͜°)ง" };
        private readonly string throwingTableString = ":(╯°□°）╯︵┻━┻";
        private readonly string user;
        private TcpClient irc;
        private StreamReader reader;
        public string Server = "irc.freenode.net";
        private readonly List<string> lastInnerException = new List<string>();
        private readonly StringMarkov markovChainModel = new StringMarkov(1);
        private bool shouldStartSavingMessages;

        public WyspaBotService(string channel, string botName) {
            this.channel = channel;
            this.botName = botName;
            this.user = $"USER {this.botName} 0 * :{this.botName}";
        }

        public void StartBot() {
            try {
                this.irc = new TcpClient(this.Server, this.port);
                var stream = this.irc.GetStream();
                this.reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                writer.WriteLine("NICK " + this.botName);
                writer.Flush();
                writer.WriteLine(this.user);
                writer.Flush();
                this.ReadChat();
            }
            catch (Exception e) {
                this.WyspaBotSay(CommandType.LogErrorCommand, "Something went wrong (>ლ)");
                this.lastInnerException.Clear();
                var excSplit = e.ToString().Split(new[] {"\r\n"}, StringSplitOptions.None);
                foreach (var m in excSplit) {
                    if (m.Contains("End of inner exception stack trace")) {
                        break;
                    }
                    this.lastInnerException.Add(m);
                }

                this.ReadChat();
            }
        }

        public void StopBot() {
            this.chatUsers.Clear();
            this.irc.Close();
        }

        private void ReadChat() {
            while (true) {
                string inputLine;
                while ((inputLine = this.reader.ReadLine()) != null) {
                    var splitInput = inputLine.Split(' ').ToList();

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
                        if (this.chatUsers.Any(x => x == previousNick)) {
                            var indexOf = this.chatUsers.IndexOf(previousNick);
                            this.chatUsers[indexOf] = splitInput[2].Substring(1, splitInput[2].Length - 1);
                        } else if (this.chatUsers.Any(x => x == $"@{previousNick}")) {
                            var indexOf = this.chatUsers.IndexOf($"@{previousNick}");
                            this.chatUsers[indexOf] = splitInput[2].Substring(1, splitInput[2].Length - 1);
                        }
                    }

                    if (splitInput.Count >= 6 && splitInput[2] == this.botName && (splitInput[3] == "@" || splitInput[3] == "=") && splitInput[4] == this.channel) {
                        var numberOfPeopleInChat = splitInput.Count - 6;
                        
                        for (var i = 0; i < numberOfPeopleInChat; i++) {
                            this.chatUsers.Add(splitInput[6 + i]);
                        }

                        this.WyspaBotSay(CommandType.SayHelloAfterJoining, this.chatUsers);
                        this.shouldStartSavingMessages = true;
                    }

                    if (splitInput[1] == "JOIN") {
                        var nick = this.GetUserNick(splitInput);
                        if (nick != this.botName) {
                            this.chatUsers.Add(nick);
                            var random = new Random();

                            var parameters = new List<string> {nick, this.channel, this.textFaces[random.Next(this.textFaces.Count)]};
                            this.WyspaBotSay(CommandType.SayHelloToNewcomerCommand, parameters);
                        }
                    }

                    if (splitInput.Count >= 3 && splitInput[1] == this.messageAlias && splitInput[2] == this.botName) {
                        var nick = this.GetUserNick(splitInput);
                        this.WyspaBotSayPrivate(CommandType.StopUsingPrivateChannelCommand, nick);
                    }

                    if (splitInput.Count >= 4 && this.shouldStartSavingMessages && splitInput[3] != $":{this.botName}" && splitInput[3] != $":{this.botName}:") {
                        var separator = $"{this.channel} :";
                        var phrase = inputLine.Substring(inputLine.IndexOf(separator, StringComparison.Ordinal) + separator.Length);
                        //this.postedMessages.Add(phrase);
                        this.markovChainModel.Learn(phrase);
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
                            case "-help":
                            case "help":
                            case "-h":
                                this.WyspaBotSay(CommandType.HelpCommand);
                                break;
                            case "-rtracks":
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
                            case "-wct":
                            case "wct":
                                this.WyspaBotSay(CommandType.TodaysWorldCupGamesCommand);
                                break;
                            case "-wcy":
                            case "wcy":
                                this.WyspaBotSay(CommandType.YesterdaysWorldCupGamesAndScoresCommand);
                                break;
                            //case "-pbin":
                            //    if (splitInput.Count >= 6) {
                            //        var numberOfMessagesToSave = splitInput[5];
                            //        var isNumber = int.TryParse(numberOfMessagesToSave, out var limit);

                            //        if (isNumber) {
                            //            var messages = postedMessages.Skip(Math.Max(0, postedMessages.Count - limit));
                            //            this.WyspaBotSay(CommandType.PasteToPastebinCommand, messages);
                            //        }
                            //        else {
                            //            this.WyspaBotSay(CommandType.LogErrorCommand, $"Provided parameter <${nameof(limit)}> is not a number");
                            //        }
                            //    }
                            //    break;
                            //case "-wiki":
                            //    var wordsToCheckInWiki = splitInput.Count - 6;
                            //    var words = new List<string>();

                            //    var numberOfWords = splitInput.Count >= wordsToCheckInWiki + 1 ? wordsToCheckInWiki + 1 : wordsToCheckInWiki;
                            //    for (var i = 0; i < numberOfWords; i++) {
                            //        words.Add(splitInput[5 + i]);
                            //    }

                            //    this.WyspaBotSay(CommandType.GetWikipediaDefinitionCommand, words);
                            //    break;
                            case "-cltmp":
                                this.postedMessages.Clear();
                                break;
                            case "-pokebattle":
                            case "pokebattle":
                                if (splitInput.Count >= 6) {
                                    var opponentName = splitInput[5];

                                    if (!this.chatUsers.Contains(opponentName)) {
                                        this.WyspaBotSay(CommandType.LogErrorCommand, $"User \"{opponentName}\" does not exist.");
                                    }
                                    else {
                                        var nick = new List<string>{this.GetUserNick(splitInput), opponentName};

                                        this.WyspaBotSay(CommandType.PokeBattleCommand, nick);
                                    }
                                }
                                else {
                                    this.WyspaBotSay(CommandType.LogErrorCommand, "You need to specify opponent's name!)");
                                }
                                break;
                            case "-ghrepo":
                            case "ghrepo":
                                this.WyspaBotSay(CommandType.GetRepositoryAddressCommand);
                                break;
                            case "-pbstats":
                            case "pbstats":
                                this.WyspaBotSay(CommandType.PokeBattleStatsCommand);
                                break;
                            case "-clpbs":
                            case "clpbs":
                                this.WyspaBotSay(CommandType.ClearPokeBattleStatsCommand);
                                break;
                            case "-gmdistance":
                            case "gmdistance":
                                if (splitInput.Count >= 7) {
                                    var origin = splitInput[5];
                                    var destination = splitInput[6];

                                    this.WyspaBotSay(CommandType.GoogleMapDistanceCommand, new List<string>{origin, destination});
                                }
                                else {
                                    this.WyspaBotSay(CommandType.LogErrorCommand, "You need to specify both: origin and destination!)");
                                }
                                break;
                            //case "-learn":
                            //case "learn":
                            //    this.markovChainModel.Learn(this.postedMessages);
                            //    this.postedMessages.Clear();
                            //    this.WyspaBotDebug(new List<string>{"OK"});
                            //    break;
                            case "-markov":
                            case "markov":
                                var message = this.markovChainModel.Walk().First();
                                this.WyspaBotDebug(new List<string>{message});
                                break;
                            case "-debug":
                                this.WyspaBotDebug(this.lastInnerException);
                                break;
                            default:
                                if (splitInput.Any(x => x.Contains(this.botName))) {
                                    this.WyspaBotSay(CommandType.ResponseWhenMentionedCommand);
                                }
                                break;
                        }
                    }
                }
            }
        }

        private string GetUserNick(IReadOnlyList<string> splitInput) {
            var indexOfExclamationMark = splitInput[0].IndexOf('!');
            return splitInput[0].Substring(1, indexOfExclamationMark - 1);
        }

        public void SendData(string cmd, string param = "") {
            if (param == null) {
                writer.WriteLine(cmd);
                writer.Flush();
            }

            else {
                writer.WriteLine(cmd + " " + param);
                writer.Flush();
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
    }
}