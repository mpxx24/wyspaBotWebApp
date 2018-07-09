using System;
using Newtonsoft.Json;

namespace wyspaBotWebApp.Services.Pokemon {
    public class PokemonApiService : IPokemonApiService {
        private readonly string apiAddress = "http://pokeapi.co/api/v2/pokemon/{0}";
        private readonly IRequestsService requestsService;

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
                //TODO: handle & log
                throw e;
            }
        }
    }
}