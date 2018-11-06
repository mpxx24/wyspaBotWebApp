using System;
using Newtonsoft.Json;
using NLog;

namespace wyspaBotWebApp.Services.Pokemon {
    public class PokemonApiService : IPokemonApiService {
        private readonly string apiAddress = "http://pokeapi.co/api/v2/pokemon/{0}";

        private readonly IRequestsService requestsService;

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public PokemonApiService(IRequestsService requestsService) {
            this.requestsService = requestsService;
        }

        public PokemonApiRootObject GetRandomPokemon() {
            try {
                var rand = new Random();
                var pokemonId = rand.Next(150);

                var pokemonJson = this.requestsService.GetData(string.Format(this.apiAddress, pokemonId));

                return JsonConvert.DeserializeObject<PokemonApiRootObject>(pokemonJson);
            }
            catch (Exception e) {
                this.logger.Debug(e, "Failed to fetch random pokemon!");
                throw e;
            }
        }
    }
}