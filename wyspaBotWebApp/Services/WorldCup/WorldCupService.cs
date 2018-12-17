using System.Collections.Generic;
using Newtonsoft.Json;

namespace wyspaBotWebApp.Services.WorldCup {
    public class WorldCupService : IWorldCupService {
        private readonly string apiAddress = "http://worldcup.sfg.io/matches";

        private readonly IRequestsService requestsService;

        public WorldCupService(IRequestsService requestsService) {
            this.requestsService = requestsService;
        }

        public IEnumerable<WorldCupApiData> GetData() {
            var data = this.requestsService.GetData(this.apiAddress);
            var wcad = JsonConvert.DeserializeObject<List<WorldCupApiData>>(data);
            return wcad;
        }
    }
}