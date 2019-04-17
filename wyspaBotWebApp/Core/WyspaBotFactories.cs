using System;

namespace wyspaBotWebApp.Core {
    public class CommandFactory {
        public ICommand GetCommand(CommandType commandType) {
            switch (commandType) {
                case CommandType.HelpCommand:
                    return new HelpCommand();
                case CommandType.PutTheTableBackCommand:
                    return new PutTheTableBackCommand();
                case CommandType.ResponseWhenMentionedCommand:
                    return new ResponseWhenMentionedCommand();
                case CommandType.TodaysWorldCupGamesCommand:
                    return new TodaysWorldCupGamesCommand();
                case CommandType.YesterdaysWorldCupGamesAndScoresCommand:
                    return new YesterdaysWorldCupGamesAndScoresCommand();
                case CommandType.RecommendedTracksBasedOnTrackCommand:
                    return new ResponseWhenMentionedCommand();
                case CommandType.StopUsingPrivateChannelCommand:
                    return new StopUsingPrivateChannelCommand();
                case CommandType.GetRepositoryAddressCommand:
                    return new GetRepositoryAddressCommand();
                case CommandType.PokeBattleStatsCommand:
                    return new PokeBattleStatsCommand();
                case CommandType.ClearPokeBattleStatsCommand:
                    return new ClearPokeBattleStatsCommand();
                case CommandType.GetNextEventCommand:
                    return new GetNextEventCommand();
                case CommandType.ListAllEventsCommand:
                    return new ListAllEventsCommand();
                case CommandType.NasaPictureOfTheDayCommand:
                    return new NasaPictureOfTheDayCommand();
                case CommandType.ResetAllEventsCommand:
                    return new ResetAllEventsCommand();
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }
    }

    public class CommandWithStringParameterFactory {
        public ICommandWithStringParameter GetCommand(CommandType commandType) {
            switch (commandType) {
                case CommandType.IntroduceYourselfCommand:
                    return new IntroduceYourselfCommand();
                case CommandType.LogErrorCommand:
                    return new LogErrorCommand();
                case CommandType.UserplaylistsCommand:
                    return new UserplaylistsCommand();
                case CommandType.GetYoutubeVideoTitleCommand:
                    return new GetYoutubeVideoTitleCommand();
                case CommandType.WolframAlphaShortQuestionCommand:
                    return new WolframAlphaShortQuestionCommand();
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }
    }

    public class CommandWithStringIenumerableParameterFactory {
        public ICommandWithStringIenumerableParameter GetCommand(CommandType commandType) {
            switch (commandType) {
                case CommandType.PasteToPastebinCommand:
                    return new PasteToPastebinCommand();
                case CommandType.SayHelloToNewcomerCommand:
                    return new SayHelloToNewcomerCommand();
                case CommandType.SayHelloAfterJoining:
                    return new SayHelloAfterJoiningCommand();;
                case CommandType.PokeBattleCommand:
                    return new PokeBattleCommand();
                case CommandType.GoogleMapDistanceCommand:
                    return new GoogleMapDistanceCommand();
                case CommandType.AddEventCommand:
                    return new AddNewEventCommand();
                case CommandType.SayHelloToAllInTheChat:
                    return new SayHelloToAllInTheChatCommand();
                case CommandType.GetMostUsedWordsCommand:
                    return new GetMostUsedWordsCommand();
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }
    }

    public class CommandWithTwoParametersFactory {
        public ICommandWithTwoParameters GetCommand(CommandType commandType) {
            switch (commandType) {
                case CommandType.RecommendedTracksBasedOnTrackCommand:
                    return new RecommendedTracksBasedOnTrackCommand();
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }
    }
}