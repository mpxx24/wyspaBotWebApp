using System.Collections.Generic;

namespace wyspaBotWebApp.Services.WorldCup {
    public interface IWorldCupService {
        IEnumerable<WorldCupApiData> GetData(string address);
    }
}