namespace wyspaBotWebApp.Core {
    public enum CommandType {
        HelpCommand = 0,
        IntroduceYourselfCommand = 1,
        PutTheTableBackCommand = 2,
        ResponseWhenMentionedCommand = 3,
        TodaysWorldCupGamesCommand = 4,
        YesterdaysWorldCupGamesAndScoresCommand = 5,
        UserplaylistsCommand = 6,
        RecommendedTracksBasedOnTrackCommand = 7,
        PasteToPastebinCommand = 8,
        SayHelloToNewcomerCommand = 9,
        SayHelloAfterJoining = 10,
        StopUsingPrivateChannelCommand = 11,
        //GetWikipediaDefinitionCommand = 12,
        PokeBattleCommand = 13,
        GetRepositoryAddressCommand = 14,
        PokeBattleStatsCommand = 15,
        ClearPokeBattleStatsCommand = 16,
        GoogleMapDistanceCommand = 17,
        AddEvent = 18,
        GetNextEvent = 19,
        ListAllEvents = 20,

        LogErrorCommand = 999,
    }
}