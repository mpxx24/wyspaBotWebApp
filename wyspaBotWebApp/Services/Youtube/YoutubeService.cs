using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NLog;

namespace wyspaBotWebApp.Services.Youtube {
    public class YoutubeService : IYoutubeService {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IRequestsService requestsService;

        private readonly string youtubeApiKey;

        private readonly string apiLink = "https://www.googleapis.com/youtube/v3/videos?part=snippet&id={0}&key={1}";

        private readonly string youtubeComeUrl = "youtube.com/";
        private readonly string youtuBeUrl = "youtu.be/";

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

        public bool IsYoutubeVideoLink(string text) {
            text = text.Trim();
            if (text.Split(' ').Length > 1) {
                return false;
            }

            return text.Contains(this.youtubeComeUrl) && text.Contains("v=") || text.Contains(this.youtuBeUrl) && this.IsShortYoutubeLink(text);
        }

        public string GetVideoId(string link) {
            link = link.Trim();
            if (link.Contains("feature=")) {
                var timeIndex = link.IndexOf("feature=", StringComparison.InvariantCulture);
                link = link.Substring(0, timeIndex - 1);
            }

            if (link.Contains("t=")) {
                var timeIndex = link.IndexOf("t=", StringComparison.InvariantCulture);
                link = link.Substring(0, timeIndex - 1);
            }

            var isYoutube = link.Contains(this.youtubeComeUrl);
            var isYoutubeShort = link.Contains(this.youtuBeUrl);

            if (isYoutube) {
                var indexFrom = link.IndexOf("v=", StringComparison.InvariantCulture);
                return link.Substring(indexFrom, link.Length - indexFrom).Replace("v=", string.Empty);
            }
            else if (isYoutubeShort) {
                var indexOfUrl = link.IndexOf(this.youtuBeUrl, StringComparison.InvariantCulture);
                return link.Substring(indexOfUrl + this.youtuBeUrl.Length, link.Length - indexOfUrl - this.youtuBeUrl.Length);
            }
            return string.Empty;
        }


        private bool IsShortYoutubeLink(string text) {
            var indexOfUrl = text.IndexOf(this.youtuBeUrl, StringComparison.InvariantCulture);
            return text.Substring(indexOfUrl, text.Length - indexOfUrl) != string.Empty;
        }
    }
}