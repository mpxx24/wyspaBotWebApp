using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyApiWrapper.API.Wrappers;
using wyspaBotWebApp.Dtos;
using wyspaBotWebApp.Services;
using wyspaBotWebApp.Services.Calendar;
using wyspaBotWebApp.Services.GoogleMaps;
using wyspaBotWebApp.Services.NasaApi;
using wyspaBotWebApp.Services.Pokemon;
using wyspaBotWebApp.Services.WorldCup;
using wyspaBotWebApp.Services.Youtube;

//TODO: move some of the logic to services
namespace wyspaBotWebApp.Core {
    public class HelpCommand : ICommand {
        public IEnumerable<string> GetText() {
            return new List<string> {
                "Commands are not case sensitive abd can be used with or without '-'",
                "",
                "Allowed commands:",
                "   help h -> show whole bot api",
                "   rtracks <track id> <limit (default: 10)> -> get tracks recommended based on given track id",
                //"   -wct -> today's world cup games",
                //"   -wcy -> yesterday's world cup games",
                //"   -pbin <numberOfLines>" -> save last <numberOfLines> messages to pastebin,
                "   ghrepo -> link to github repository",
                "   pokebattle <nick> -> pokemon battle",
                "   pbstats -> pokemon battle stats",
                "   markov -> generate text from string markov",
                "   addevent <name> - <place> <date/number of days from now> -> add new event",
                "   listevents -> list all added events (from the future)",
                "   nextevent -> list next event (closest in time)",
                "   npod -> NASA's picture of the day",
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
            return new List<string> {"https://github.com/mpxx24/wyspaBotWebApp"};
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
            return new List<string> {"Stats has been cleared."};
        }
    }

    public class GetNextEventCommand : ICommand {
        public IEnumerable<string> GetText() {
            var nextEntry = IoC.Resolve<ICalendarService>().GetNextEntry();
            return nextEntry == null
                ? new List<string> {"No events defined!"}
                : new List<string> {$"{nextEntry.Name} - {nextEntry.Place} - {nextEntry.When.ToString(ApplicationSettingsHelper.DateTimeFormat)}"};
        }
    }

    public class ListAllEventsCommand : ICommand {
        public IEnumerable<string> GetText() {
            var allEntries = IoC.Resolve<ICalendarService>().GetAllEntries();
            return allEntries.Select(x => $"{x.Name} - {x.Place} - {x.When.ToString(ApplicationSettingsHelper.DateTimeFormat)}");
        }
    }

    public class NasaPictureOfTheDayCommand : ICommand {
        public IEnumerable<string> GetText() {
            var pictoreOfTheDayRootObject = IoC.Resolve<INasaApiService>().GetPictureOfTheDay();
            return new List<string> {
                pictoreOfTheDayRootObject.Title,
                pictoreOfTheDayRootObject.Hdurl.ToString()
            };
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
    public class GetYoutubeVideoTitleCommand : ICommandWithStringParameter {
        public IEnumerable<string> GetText(string parameter) {
            if (string.IsNullOrEmpty(parameter)) {
                return new List<string> {"Link not provided!"};
            }

            var title = IoC.Resolve<IYoutubeService>().GetVideoName(parameter);
            return new List<string>{title};
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
    public class GoogleMapDistanceCommand : ICommandWithStringIenumerableParameter {
        public IEnumerable<string> GetText(IEnumerable<string> parameters) {
            var p = parameters as IList<string> ?? parameters.ToList();

            return IoC.Resolve<IGoogleMapsService>().GetDistance(p[0], p[1]);
        }
    }

    //TODO: refactor
    public class AddNewEventCommand : ICommandWithStringIenumerableParameter {
        public IEnumerable<string> GetText(IEnumerable<string> parameters) {
            var p = parameters as IList<string> ?? parameters.ToList();

            if (p.Count != 4) {
                return new List<string> {"Invalid number of parameters!"};
            }

            var when = new DateTime();
            if (!DateTime.TryParse(p[3], out when)) {
                if (int.TryParse(p[3], out var numberOfDaysFromNow)) {
                    when = DateTime.Now.AddDays(numberOfDaysFromNow);
                }
            }

            var calendatDto = new CalendarEventDto {
                Id = Guid.NewGuid(),
                AddedBy = p[0],
                Added = DateTime.Now,
                Name = p[1],
                Place = p[2],
                When = when
            };
            IoC.Resolve<ICalendarService>().AddEntry(calendatDto);
            return new List<string> {"New event added!"};
        }
    }

    public class SayHelloToAllInTheChatCommand : ICommandWithStringIenumerableParameter {
        public IEnumerable<string> GetText(IEnumerable<string> parameters) {
            var p = parameters as IList<string> ?? parameters.ToList();

            return new List<string> {$"Hello {string.Join(" ", p)}"};
        }
    }

    //TODO: refactor
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