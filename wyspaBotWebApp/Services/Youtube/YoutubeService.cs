using System;
using Newtonsoft.Json;
using NLog;

namespace wyspaBotWebApp.Services.Youtube {
    public class YoutubeService : IYoutubeService {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IRequestsService requestsService;

        private readonly string youtubeApiKey;

        private readonly string apiLink = "https://www.googleapis.com/youtube/v3/videos?part=snippet&id={0}&key={1}";

        public YoutubeService(IRequestsService requestsService, string youtubeApiKey) {
            this.requestsService = requestsService;
            this.youtubeApiKey = youtubeApiKey;
        }

        public string GetVideoName(string link) {
            try {
                var id = this.GetVideoId(link);

                var response = this.requestsService.GetData(string.Format(this.apiLink, id, this.youtubeApiKey));
                var responseAsObject = JsonConvert.DeserializeObject<YoutubeApiRootObject>(response);
                return responseAsObject?.Items[0]?.Snippet?.Title ?? string.Empty;
            }
            catch (Exception e) {
                this.logger.Debug(e, $"Failed to retrieve youtube video metadata! Link: {link}");
                throw;
            }
        }

        public bool IsYoutubeLink(string text) {
            return (text.Contains("youtube.com") || text.Contains("youtu.be")) && text.Contains("v=");
        }

        private string GetVideoId(string link) {
            if (link.Contains("t=")) {
                var timeIndex = link.IndexOf("t=", StringComparison.InvariantCulture);
                link = link.Substring(0, timeIndex - 1);
            }

            var indexFrom = link.IndexOf("v=", StringComparison.InvariantCulture);
            return link.Substring(indexFrom, link.Length - indexFrom).Replace("v=", string.Empty);
        }
    }
}