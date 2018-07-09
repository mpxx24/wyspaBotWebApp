using System.Collections.Generic;
using wyspaBotWebApp.Services.WorldCup;

namespace wyspaBotWebApp.Services {
    public interface IWorldCupService {
        IEnumerable<WorldCupApiData> GetData(string address);
    }
}