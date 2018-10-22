using System.Collections.Generic;
using Google.Maps;
using Google.Maps.DistanceMatrix;

namespace wyspaBotWebApp.Services.GoogleMaps
{
    public class GoogleMapsService : IGoogleMapsService {
        public IEnumerable<string> GetDistance(string origin, string destination) {
            var distanceMatrixRequest = new DistanceMatrixRequest();
            distanceMatrixRequest.AddOrigin(origin);
            distanceMatrixRequest.AddDestination(destination);
            distanceMatrixRequest.Mode = TravelMode.driving;

            var response = new DistanceMatrixService().GetResponse(distanceMatrixRequest);

            return new List<string> {
                response?.Rows[0]?.Elements[0]?.distance?.Text ?? "Failed to complete operation"
            };
        }
    }
}