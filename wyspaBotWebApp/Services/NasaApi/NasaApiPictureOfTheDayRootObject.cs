using System;
using Newtonsoft.Json;

namespace wyspaBotWebApp.Services.NasaApi {
    public class NasaApiPictureOfTheDayRootObject {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("explanation")]
        public string Explanation { get; set; }

        [JsonProperty("hdurl")]
        public Uri Hdurl { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("service_version")]
        public string ServiceVersion { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}