using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifyApiWrapper.API.Wrappers;
using wyspaBotWebApp.Services;
using wyspaBotWebApp.Services.Pokemon;
using WikipediaApi;
using WikipediaApi.Helpers;

//TODO: move some of the logic to services
namespace wyspaBotWebApp.Core {
    public class HelpCommand : ICommand {
        public IEnumerable<string> GetText() {
            return new List<string> {
                "Allowed commands:",
                "   -help -h -> show whole bot api",
                "   -rtracks <track id> <limit (default: 10)> -> get tracks recommended based on given track id",
                "   -wct -> today's world cup games",
                "   -wcy -> yesterday's world cup games",
                //"   -wiki <text> <lang (PL/EN)>-> wikipedia's definition (BETA :P)",
                //"   -pbin <numberOfLines>" -> save last <numberOfLines> messages to pastebin,
                "   -ghrepo link to github repository",
                "",
                "Throwing tables is not allowed!"
            };
        }
    }

    public class PutTheTableBackCommand : ICommand {
        public IEnumerable<string> GetText() {
            return new List<string> {
                "",
                "（╯°□°）╯︵( .o.)",
                "┬──┬ ノ( ゜-゜ノ)",
                ""
            };
        }
    }

    public class ResponseWhenMentionedCommand : ICommand {
        public IEnumerable<string> GetText() {
            return new List<string> {
                "¯\\_(ツ)_/¯"
            };
        }
    }

    public class TodaysWorldCupGamesCommand : ICommand {
        public IEnumerable<string> GetText() {
            var worldCupData = IoC.Resolve<IWorldCupService>().GetData("http://worldcup.sfg.io/matches");

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            return worldCupData
                .Where(x => x.Datetime.Date == DateTime.Today)
                .OrderBy(x => x.Datetime)
                .Select(y => $"{y.HomeTeam.Country} - {y.AwayTeam.Country} ({TimeZoneInfo.ConvertTimeFromUtc(y.Datetime.DateTime, timeZoneInfo)})");
        }
    }

    public class YesterdaysWorldCupGamesAndScoresCommand : ICommand {
        public IEnumerable<string> GetText() {
            var worldCupData = IoC.Resolve<IWorldCupService>().GetData("http://worldcup.sfg.io/matches");

            return worldCupData
                .Where(x => x.Datetime.Date == DateTime.Today.AddDays(-1))
                .OrderBy(x => x.Datetime)
                .Select(y => $"{y.HomeTeam.Country} {y.HomeTeam.Goals} - {y.AwayTeam.Goals} {y.AwayTeam.Country}");
        }
    }

    public class StopUsingPrivateChannelCommand : ICommand {
        public IEnumerable<string> GetText() {
            return new List<string> {"Stop using private messages!"};
        }
    }

    public class GetRepositoryAddressCommand : ICommand {
        public IEnumerable<string> GetText() {
            return new List<string> { "https://github.com/mpxx24/wyspaBotWebApp" };
        }
    }

    public class PokeBattleStatsCommand : ICommand {
        public IEnumerable<string> GetText() {
            return IoC.Resolve<IPokemonService>().GetPokeBattleStats();
        }
    }
    public class ClearPokeBattleStatsCommand : ICommand {
        public IEnumerable<string> GetText() {
            IoC.Resolve<IPokemonService>().ClearStats();
            return new List<string>{"Stats has been cleared."};
        }
    }

    public class IntroduceYourselfCommand : ICommandWithStringParameter {
        public IEnumerable<string> GetText(string parameter) {
            return new List<string> {
                "my name is bot",
                $"{parameter} ( ͡° ͜ʖ ͡°)"
            };
        }
    }

    public class LogErrorCommand : ICommandWithStringParameter {
        public IEnumerable<string> GetText(string parameter) {
            return new List<string> {parameter};
        }
    }

    public class UserplaylistsCommand : ICommandWithStringParameter {
        public IEnumerable<string> GetText(string parameter) {
            if (string.IsNullOrEmpty(parameter)) {
                return new List<string> {"Username not provided. Use -help/-h for more info"};
            }

            var playlists = PlaylistsWrapper.GetUsersPlaylists(parameter);
            return playlists.Select(x => x.Name);
        }
    }

    public class PokeBattleCommand : ICommandWithStringIenumerableParameter {
        public IEnumerable<string> GetText(IEnumerable<string> parameter) {
            var listOfNames = parameter as IList<string> ?? parameter.ToList();
            return listOfNames.Count != 2 
                ? new List<string> {$"Inconsistent data. Expected 2 parameters but got {listOfNames.Count}"} 
                : IoC.Resolve<IPokemonService>().PerformBattle(listOfNames.First(), listOfNames.Last());
        }
    }

    public class PasteToPastebinCommand : ICommandWithStringIenumerableParameter {
        public IEnumerable<string> GetText(IEnumerable<string> messages) {
            var link = IoC.Resolve<IPasteBinApiService>().Save(messages);
            return new List<string> {link};
        }
    }

    //TODO: refactor
    public class SayHelloToNewcomerCommand : ICommandWithStringIenumerableParameter {
        public IEnumerable<string> GetText(IEnumerable<string> parameters) {
            var p = parameters as IList<string> ?? parameters.ToList();
            return p.Count >= 3
                ? new List<string> {$"Hi {p[0]} :) Welcome to channel {p[1]} {p[2]}"}
                : new List<string> {"Hi! Welcome to chat :)"};
        }
    }

    //TODO: refactor
    public class SayHelloAfterJoiningCommand : ICommandWithStringIenumerableParameter {
        public IEnumerable<string> GetText(IEnumerable<string> nicks) {
            var listOfNicks = nicks as IList<string> ?? nicks.ToList();

            return listOfNicks.Select(nick => $"Hello {nick} :)").ToList();
        }
    }

    //TODO: refactor
    public class GetWikipediaDefinitionCommand : ICommandWithStringIenumerableParameter {
        public IEnumerable<string> GetText(IEnumerable<string> items) {
            var listOfItems = items as IList<string> ?? items.ToList();

            WikiLanguage language;
            switch (listOfItems.Last()) {
                case "PL":
                    language = WikiLanguage.Polish;
                    break;
                case "EN":
                    language = WikiLanguage.English;
                    break;
                default:
                    language = WikiLanguage.Polish;
                    listOfItems.Add("PL");
                    break;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < listOfItems.Count - 1; i++) {
                sb.Append($"{listOfItems[i]} ");
            }

            sb.Length--;

            string article;
            try {
                article = WikiApi.GetWikipediaArticle(sb.ToString(), language);
            }
            catch (Exception e) {
                article = "failed to retrieve data";
            }

            return new List<string> {article};
        }
    }

    public class RecommendedTracksBasedOnTrackCommand : ICommandWithTwoParameters {
        public IEnumerable<string> GetText(string parameter, int value) {
            if (string.IsNullOrEmpty(parameter)) {
                return new List<string> {"Track ID not provided. Use -help/-h for more info"};
            }
            var numberOfTracks = 10;
            if (value != 0) {
                numberOfTracks = value;
            }

            var tracks = RecommendationsWrapper.GetTracksReccomendationsBasedOnTrack(parameter, numberOfTracks);
            return tracks.Select(x => $"{string.Join("&", x.Artists.Select(y => y.Name))} - {x.Name}");
        }
    }
}