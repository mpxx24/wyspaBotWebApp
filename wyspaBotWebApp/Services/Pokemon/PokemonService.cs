using System;
using System.Collections.Generic;
using System.Linq;

namespace wyspaBotWebApp.Services.Pokemon {
    public class PokemonService : IPokemonService {
        private readonly IPokemonApiService pokemonApiService;

        public PokemonService(IPokemonApiService pokemonApiService) {
            this.pokemonApiService = pokemonApiService;
        }

        public IEnumerable<string> PerformBattle(string challengerName, string opponentName) {
            var battle = new List<string> {$"{challengerName} and {opponentName} - it's time to d-d-d-d-d-duel!"};

            var pokemonOfFirstPLayer = this.GetRandomPokemonForPlayer();
            var pokemonOfSecondPlayer = this.GetRandomPokemonForPlayer();

            if (pokemonOfFirstPLayer.id == pokemonOfSecondPlayer.id) {
                while (pokemonOfSecondPlayer.id == pokemonOfFirstPLayer.id) {
                    pokemonOfSecondPlayer = this.GetRandomPokemonForPlayer();
                }
            }

            battle.Add($"{challengerName}'s pokemon is {pokemonOfFirstPLayer.name}! - {opponentName}'s pokemon is {pokemonOfSecondPlayer.name}!");
            battle.Add("FIGHT!");

            var rand = new Random();

            var firstPokemonsHealth = pokemonOfFirstPLayer.stats?.FirstOrDefault(x => x.stat.name == "hp")?.base_stat;
            var secondPokemonsHealth = pokemonOfSecondPlayer.stats?.FirstOrDefault(x => x.stat.name == "hp")?.base_stat;

            battle.Add($"{pokemonOfFirstPLayer.name}'s HP: {firstPokemonsHealth} - {pokemonOfSecondPlayer.name}'s HP: {secondPokemonsHealth}");

            while (firstPokemonsHealth > 0 && secondPokemonsHealth > 0) {
                var randAbilityNameForFirstPokemon = GetRandomAbilityNameForPokemon(pokemonOfFirstPLayer);
                var randAbilityNameForSecondPokemon = GetRandomAbilityNameForPokemon(pokemonOfSecondPlayer);

                battle.Add($"{pokemonOfFirstPLayer.name} uses {randAbilityNameForFirstPokemon}!");
                battle.Add($"{pokemonOfSecondPlayer.name} uses {randAbilityNameForSecondPokemon}!");

                var firstPokemonAbilityDmg = rand.Next(secondPokemonsHealth.Value + 1);
                var secondPokemonAbilityDmg = rand.Next(firstPokemonsHealth.Value + 1);
                
                if (rand.Next(100) % 30 == 0) {
                    firstPokemonAbilityDmg *= 2;
                    secondPokemonsHealth -= firstPokemonAbilityDmg;
                    battle.Add($"CRIT! - {randAbilityNameForFirstPokemon} dealt {firstPokemonAbilityDmg} DMG!");
                }
                else {
                    secondPokemonsHealth -= firstPokemonAbilityDmg;
                    battle.Add($"{randAbilityNameForFirstPokemon} dealt {firstPokemonAbilityDmg} DMG!");
                }

                if (rand.Next(100) % 30 == 0) {
                    secondPokemonAbilityDmg *= 2;
                    firstPokemonsHealth -= secondPokemonAbilityDmg;
                    battle.Add($"CRIT! - {randAbilityNameForSecondPokemon} dealt {secondPokemonAbilityDmg} DMG!");
                }
                else {
                    firstPokemonsHealth -= secondPokemonAbilityDmg;
                    battle.Add($"{randAbilityNameForSecondPokemon} dealt {secondPokemonAbilityDmg} DMG!");
                }

                battle.Add($"{pokemonOfFirstPLayer.name}'s HP: {firstPokemonsHealth} - {pokemonOfSecondPlayer.name}'s HP: {secondPokemonsHealth}");
            }

            if (firstPokemonsHealth <= 0 && secondPokemonsHealth <= 0) {
                battle.Add("TIE!");
            }
            else {
                var winnersName = firstPokemonsHealth > secondPokemonsHealth ? pokemonOfFirstPLayer.name : pokemonOfSecondPlayer.name;

                battle.Add($"{winnersName} WINS!");
            }

            return battle;
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
    }
}