using System.Collections.Generic;
using Newtonsoft.Json;
using wyspaBotWebApp.Models;

namespace wyspaBotWebApp.Services {
    public class WorldCupService : IWorldCupService {
        private readonly IRequestsService requestsService;

        public WorldCupService(IRequestsService requestsService) {
            this.requestsService = requestsService;
        }

        public IEnumerable<WorldCupApiData> GetData(string address) {
            var data = this.requestsService.GetData(address);
            var wcad = JsonConvert.DeserializeObject<List<WorldCupApiData>>(data);
            return wcad;
        }
    }
}