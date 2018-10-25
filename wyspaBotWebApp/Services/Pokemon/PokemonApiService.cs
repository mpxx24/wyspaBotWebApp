using System;
using Newtonsoft.Json;
using NLog;

namespace wyspaBotWebApp.Services.Pokemon {
    public class PokemonApiService : IPokemonApiService {
        private readonly string apiAddress = "http://pokeapi.co/api/v2/pokemon/{0}";
        private readonly IRequestsService requestsService;

        private ILogger logger = LogManager.GetCurrentClassLogger();

        public PokemonApiService(IRequestsService requestsService) {
            this.requestsService = requestsService;
        }

        public PokemonApiRootObject GetRandomPokemon() {
            var rand = new Random();
            var pokemonId = rand.Next(150);

            var pokemonJson = this.requestsService.GetData(string.Format(this.apiAddress, pokemonId));

            try {
                return JsonConvert.DeserializeObject<PokemonApiRootObject>(pokemonJson);
            }
            catch (Exception e) {
                this.logger.Debug(e, $"Failed to fetch random pokemon! Pokemon ID {pokemonId}. The response was: {pokemonJson}");
                throw e;
            }
        }
    }
}