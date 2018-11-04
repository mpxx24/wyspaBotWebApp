using System;
using Newtonsoft.Json;
using NLog;

namespace wyspaBotWebApp.Services.NasaApi {
    public class NasaApiService : INasaApiService {
        private readonly string pictureOfTheDayUrl = "https://api.nasa.gov/planetary/apod?api_key={0}";

        private readonly string apiKey;

        private readonly IRequestsService requestsService;

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public NasaApiService(IRequestsService requestsService, string apiKey) {
            this.requestsService = requestsService;
            this.apiKey = apiKey;
        }

        public NasaApiPictureOfTheDayRootObject GetPictureOfTheDay() {
            var data = this.requestsService.GetData(string.Format(this.pictureOfTheDayUrl, this.apiKey));
            try {
                return JsonConvert.DeserializeObject<NasaApiPictureOfTheDayRootObject>(data);
            }
            catch (Exception e) {
                this.logger.Debug(e, "Failed to fetch NASA's picture of the day!");
                throw;
            }
        }
    }
}