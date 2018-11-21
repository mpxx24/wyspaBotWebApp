using System;
using System.Collections.Generic;
using Google.Maps;
using Google.Maps.DistanceMatrix;
using NLog;

namespace wyspaBotWebApp.Services.GoogleMaps
{
    public class GoogleMapsService : IGoogleMapsService {
        private ILogger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<string> GetDistance(string origin, string destination) {
            try {
                var distanceMatrixRequest = new DistanceMatrixRequest();
                distanceMatrixRequest.AddOrigin(origin);
                distanceMatrixRequest.AddDestination(destination);
                distanceMatrixRequest.Mode = TravelMode.driving;

                var response = new DistanceMatrixService().GetResponse(distanceMatrixRequest);

                return new List<string> {
                    response?.Rows[0]?.Elements[0]?.distance?.Text ?? "Failed to complete operation"
                };
            }
            catch (Exception e) {
                this.logger.Debug($"Failed to get distance between {origin} and {destination}. {e}");
                throw;
            }
           
        }
    }
}