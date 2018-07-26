namespace wyspaBotWebApp.Services.Pokemon.Dtos {
    public class PokeBattleStatsDto {
        public PokeBattleStandingsDto PlayersWithTheMostWins { get; set; }
        public PokeBattleStandingsDto PokemonsWithTheMostWins { get; set; }
        public PokeBattleStandingsDto PlayersWithThewMostGames { get; set; }
        public PokeBattleStandingsDto PlayersThatStartedTheMostGames { get; set; }
        public PokeBattleStandingsDto PlayersThatWereChallengedMostOften { get; set; }
    }
}