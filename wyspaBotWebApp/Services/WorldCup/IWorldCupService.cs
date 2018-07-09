using System.Collections.Generic;
using wyspaBotWebApp.Models;

namespace wyspaBotWebApp.Services {
    public interface IWorldCupService {
        IEnumerable<WorldCupApiData> GetData(string address);
    }
}