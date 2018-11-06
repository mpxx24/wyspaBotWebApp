using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Models;
using wyspaBotWebApp.Services.Pokemon.Dtos;

namespace wyspaBotWebApp.Services.Pokemon {
    public class PokemonService : IPokemonService {
        private readonly IRepository<PokeBattleResult> pokeBattleRepository;

        private readonly IPokemonApiService pokemonApiService;

        private ILogger logger = LogManager.GetCurrentClassLogger();

        public PokemonService(IPokemonApiService pokemonApiService, IRepository<PokeBattleResult> pokeBattleRepository) {
            this.pokemonApiService = pokemonApiService;
            this.pokeBattleRepository = pokeBattleRepository;
        }

        public IEnumerable<string> PerformBattle(string challengerName, string opponentName) {
            try {

                if (challengerName == opponentName) {
                    return new List<string> {"You can't fight yourself..."};
                }

                var fullBattle = new List<string> {$"{challengerName} and {opponentName} - it's time to d-d-d-d-d-duel!"};
                var pokeBattleRecord = new PokeBattleResult();

                var pokemonOfFirstPLayer = this.GetRandomPokemonForPlayer();
                var pokemonOfSecondPlayer = this.GetRandomPokemonForPlayer();

                if (pokemonOfFirstPLayer.id == pokemonOfSecondPlayer.id) {
                    while (pokemonOfSecondPlayer.id == pokemonOfFirstPLayer.id) {
                        pokemonOfSecondPlayer = this.GetRandomPokemonForPlayer();
                    }
                }

                fullBattle.Add($"{challengerName}'s pokemon is {pokemonOfFirstPLayer.name}! - {opponentName}'s pokemon is {pokemonOfSecondPlayer.name}!");
                fullBattle.Add("FIGHT!");

                var rand = new Random();

                var firstPokemonsHealth = pokemonOfFirstPLayer.stats?.FirstOrDefault(x => x.stat.name == "hp")?.base_stat;
                var secondPokemonsHealth = pokemonOfSecondPlayer.stats?.FirstOrDefault(x => x.stat.name == "hp")?.base_stat;

                fullBattle.Add($"{pokemonOfFirstPLayer.name}'s HP: {firstPokemonsHealth} - {pokemonOfSecondPlayer.name}'s HP: {secondPokemonsHealth}");

                var battle = new StringBuilder();
                while (firstPokemonsHealth > 0 && secondPokemonsHealth > 0) {
                    var firstPokemonAbilityDmg = rand.Next(secondPokemonsHealth.Value + 1);
                    var secondPokemonAbilityDmg = rand.Next(firstPokemonsHealth.Value + 1);

                    if (rand.Next(100) % 30 == 0) {
                        firstPokemonAbilityDmg *= 2;
                        secondPokemonsHealth -= firstPokemonAbilityDmg;
                    }
                    else {
                        secondPokemonsHealth -= firstPokemonAbilityDmg;
                    }

                    if (rand.Next(100) % 30 == 0) {
                        secondPokemonAbilityDmg *= 2;
                        firstPokemonsHealth -= secondPokemonAbilityDmg;
                    }
                    else {
                        firstPokemonsHealth -= secondPokemonAbilityDmg;
                    }

                    battle.Append($"({firstPokemonsHealth} - {secondPokemonsHealth}),");
                }
                battle.Length -= 1;
                fullBattle.Add(battle.ToString());

                if (firstPokemonsHealth <= 0 && secondPokemonsHealth <= 0) {
                    pokeBattleRecord.PlayerNick = challengerName;
                    pokeBattleRecord.OpponentNick = opponentName;
                    pokeBattleRecord.WinnerNick = string.Empty;
                    pokeBattleRecord.WinningPokemonName = string.Empty;
                    pokeBattleRecord.LoosingPokemonName = string.Empty;

                    fullBattle.Add("TIE!");
                }
                else {
                    var winner = firstPokemonsHealth > secondPokemonsHealth ? challengerName : opponentName;
                    var winningPokemon = firstPokemonsHealth > secondPokemonsHealth ? pokemonOfFirstPLayer : pokemonOfSecondPlayer;

                    pokeBattleRecord.PlayerNick = challengerName;
                    pokeBattleRecord.OpponentNick = opponentName;
                    pokeBattleRecord.WinnerNick = winner;
                    pokeBattleRecord.WinningPokemonName = winningPokemon.name;
                    pokeBattleRecord.LoosingPokemonName = winningPokemon == pokemonOfFirstPLayer ? pokemonOfSecondPlayer.name : pokemonOfFirstPLayer.name;

                    fullBattle.Add($"{winner} and his {winningPokemon.name} WON!");
                }

                this.pokeBattleRepository.Save(pokeBattleRecord);

                return fullBattle;
            }
            catch (Exception e) {
                this.logger.Debug(e, "Exception occured while perfoming poke battle");
                throw;
            }
        }

        public IEnumerable<string> GetPokeBattleStats() {
            var allBattles = this.pokeBattleRepository.GetAll().ToList();
            var results = this.GetPokeBattleStatsBasedOnRecords(allBattles);

            return new List<string> {
                "POKEBATTLE STATS v1:",
                " ",
                $"MOST WINS PER PLAYER: ({results.PlayersWithTheMostWins.FirstPlace.Value}) {results.PlayersWithTheMostWins.FirstPlace.Name}, ({results.PlayersWithTheMostWins.SecondPlace.Value}) {results.PlayersWithTheMostWins.SecondPlace.Name}, ({results.PlayersWithTheMostWins.ThirdPlace.Value}) {results.PlayersWithTheMostWins.ThirdPlace.Name}",
                $"MOST WINS PER POKEMON: ({results.PokemonsWithTheMostWins.FirstPlace.Value}) {results.PokemonsWithTheMostWins.FirstPlace.Name}, ({results.PokemonsWithTheMostWins.SecondPlace.Value}) {results.PokemonsWithTheMostWins.SecondPlace.Name}, ({results.PokemonsWithTheMostWins.ThirdPlace.Value}) {results.PokemonsWithTheMostWins.ThirdPlace.Name}",
                $"MOST GAMES TOTAL PER PLAYER: ({results.PlayersWithThewMostGames.FirstPlace.Value}) {results.PlayersWithThewMostGames.FirstPlace.Name}, ({results.PlayersWithThewMostGames.SecondPlace.Value}) {results.PlayersWithThewMostGames.SecondPlace.Name}, ({results.PlayersWithThewMostGames.ThirdPlace.Value}) {results.PlayersWithThewMostGames.ThirdPlace.Name}",
                $"MOST GAMES STARTED PER PLAYER: ({results.PlayersThatStartedTheMostGames.FirstPlace.Value}) {results.PlayersThatStartedTheMostGames.FirstPlace.Name}, ({results.PlayersThatStartedTheMostGames.SecondPlace.Value}) {results.PlayersThatStartedTheMostGames.SecondPlace.Name}, ({results.PlayersThatStartedTheMostGames.ThirdPlace.Value}) {results.PlayersThatStartedTheMostGames.ThirdPlace.Name}",
                $"MOST BELOVED OPPENENT PER PLAYER: ({results.PlayersThatWereChallengedMostOften.FirstPlace.Value}) {results.PlayersThatWereChallengedMostOften.FirstPlace.Name}, ({results.PlayersThatWereChallengedMostOften.SecondPlace.Value}) {results.PlayersThatWereChallengedMostOften.SecondPlace.Name}, ({results.PlayersThatWereChallengedMostOften.ThirdPlace.Value}) {results.PlayersThatWereChallengedMostOften.ThirdPlace.Name}",
                " "
            };
        }

        public void ClearStats() {
            this.logger.Debug("Clearing poke battle stats");
            this.pokeBattleRepository.DeleteAll();
        }

        private static string GetRandomAbilityNameForPokemon(PokemonApiRootObject pokemon) {
            var rand = new Random();
            var randAbilityNumberForFirstPokemon = rand.Next(pokemon.abilities.Count);
            var randAbilituNameForFirstPokemon = pokemon.abilities[randAbilityNumberForFirstPokemon].ability.name;
            return randAbilituNameForFirstPokemon;
        }

        private PokemonApiRootObject GetRandomPokemonForPlayer() {
            return this.pokemonApiService.GetRandomPokemon();
        }

        private PokeBattleStatsDto GetPokeBattleStatsBasedOnRecords(IEnumerable<PokeBattleResult> results) {
            var pokeBattleResults = results as IList<PokeBattleResult> ?? results.ToList();

            var numberOfGamesWonPerPlayer = pokeBattleResults.Where(x => !string.IsNullOrEmpty(x.WinnerNick)).GroupBy(x => x.WinnerNick)
                                                             .Select(gr => new {name = gr.Key, number = gr.Count()})
                                                             .OrderByDescending(y => y.number)
                                                             .Select(pokeBattleResult => new PokeBattleStringNumberObject(pokeBattleResult.number, pokeBattleResult.name))
                                                             .ToList();

            var numberOfGamesWonPerPokemon = pokeBattleResults.Where(x => !string.IsNullOrEmpty(x.WinningPokemonName)).GroupBy(x => x.WinningPokemonName)
                                                              .Select(gr => new {name = gr.Key, number = gr.Count()})
                                                              .OrderByDescending(y => y.number)
                                                              .Select(pokeBattleResult => new PokeBattleStringNumberObject(pokeBattleResult.number, pokeBattleResult.name))
                                                              .ToList();

            var numberOfGamesStartedPerPlayer = pokeBattleResults.Where(x => !string.IsNullOrEmpty(x.PlayerNick)).GroupBy(x => x.PlayerNick)
                                                                 .Select(gr => new {name = gr.Key, numberOfWins = gr.Count()})
                                                                 .OrderByDescending(y => y.numberOfWins)
                                                                 .Select(pokeBattleResult => new PokeBattleStringNumberObject(pokeBattleResult.numberOfWins, pokeBattleResult.name))
                                                                 .ToList();

            var numberOfGamesBeingChallengedPerPlayer = pokeBattleResults.Where(x => !string.IsNullOrEmpty(x.OpponentNick)).GroupBy(x => x.OpponentNick)
                                                                         .Select(gr => new {name = gr.Key, numberOfWins = gr.Count()})
                                                                         .OrderByDescending(y => y.numberOfWins)
                                                                         .Select(pokeBattleResult => new PokeBattleStringNumberObject(pokeBattleResult.numberOfWins, pokeBattleResult.name))
                                                                         .ToList();

            //need to copy by value...
            var totalNumberOfGamesPerPLayer = new List<PokeBattleStringNumberObject>(numberOfGamesStartedPerPlayer);
            foreach (var ch in numberOfGamesBeingChallengedPerPlayer) {
                if (totalNumberOfGamesPerPLayer.Any(x => x.Name == ch.Name)) {
                    totalNumberOfGamesPerPLayer.First(y => ch.Name != null && y.Name == ch.Name).Value += ch.Value;
                }
                else {
                    totalNumberOfGamesPerPLayer.Add(new PokeBattleStringNumberObject(ch.Value, ch.Name));
                }
            }
            totalNumberOfGamesPerPLayer = totalNumberOfGamesPerPLayer.OrderByDescending(x => x.Value).ToList();

            var stats = new PokeBattleStatsDto {
                PlayersWithTheMostWins = this.GetStandingFromList(numberOfGamesWonPerPlayer),
                PlayersThatStartedTheMostGames = this.GetStandingFromList(numberOfGamesStartedPerPlayer),
                PlayersThatWereChallengedMostOften = this.GetStandingFromList(numberOfGamesBeingChallengedPerPlayer),
                PlayersWithThewMostGames = this.GetStandingFromList(totalNumberOfGamesPerPLayer),
                PokemonsWithTheMostWins = this.GetStandingFromList(numberOfGamesWonPerPokemon)
            };

            return stats;
        }

        private PokeBattleStandingsDto GetStandingFromList(IList<PokeBattleStringNumberObject> list) {
            var newElement = new PokeBattleStringNumberObject(0, string.Empty);

            switch (list.Count) {
                case 0:
                    return new PokeBattleStandingsDto {FirstPlace = newElement, SecondPlace = newElement, ThirdPlace = newElement};
                case 1:
                    return new PokeBattleStandingsDto {FirstPlace = list[0], SecondPlace = newElement, ThirdPlace = newElement};
                case 2:
                    return new PokeBattleStandingsDto {FirstPlace = list[0], SecondPlace = list[1], ThirdPlace = newElement};
                default:
                    return new PokeBattleStandingsDto {FirstPlace = list[0], SecondPlace = list[1], ThirdPlace = list[2]};
            }
        }
    }
}