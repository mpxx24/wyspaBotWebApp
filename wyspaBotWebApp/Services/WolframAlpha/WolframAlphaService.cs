using System;
using System.Web;
using NLog;

namespace wyspaBotWebApp.Services.WolframAlpha {
    public class WolframAlphaService : IWolframAlphaService {
        private readonly string apiAddress = "http://api.wolframalpha.com/v1/result?appid={0}";

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IRequestsService requestsService;

        private readonly string wolframAlphaAppId;

        public WolframAlphaService(string wolframAlphaAppId, IRequestsService requestsService) {
            this.wolframAlphaAppId = wolframAlphaAppId;
            this.requestsService = requestsService;
        }

        public string GetShortAnswer(string question) {
            try {
                var address = string.Format(this.apiAddress, this.wolframAlphaAppId);
                var data = this.requestsService.GetData($"{address}&i={HttpUtility.UrlEncode(question)}");
                return data;
            }
            catch (Exception e) {
                this.logger.Debug($"Failed to fetch short answer for question '{question}'. {e}");
                throw;
            }
        }
    }
}