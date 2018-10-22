using System.Collections.Generic;

namespace wyspaBotWebApp.Services.GoogleMaps {
    public interface IGoogleMapsService {
        IEnumerable<string> GetDistance(string origin, string destination);
    }
}