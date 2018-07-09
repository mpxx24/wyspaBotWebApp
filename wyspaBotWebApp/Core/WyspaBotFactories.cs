﻿using System;

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
                case CommandType.GetWikipediaDefinitionCommand:
                    return new GetWikipediaDefinitionCommand();
                case CommandType.PokeBattleCommand:
                    return new PokeBattleCommand();
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