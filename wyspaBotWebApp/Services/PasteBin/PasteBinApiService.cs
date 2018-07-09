using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace wyspaBotWebApp.Services {
    public class PasteBinApiService : IPasteBinApiService {
        private readonly string loginUrl = "http://pastebin.com/api/api_login.php";
        private readonly string pastebinApiDevKey;
        private readonly IRequestsService requestsService;

        public PasteBinApiService(string pastebinApiDevKey, IRequestsService requestsService) {
            this.pastebinApiDevKey = pastebinApiDevKey;
            this.requestsService = requestsService;
        }

        public string Save(IEnumerable<string> messages) {
            var sb = new StringBuilder();

            foreach (var message in messages) {
                sb.AppendLine(message);
            }

            var parameters = new NameValueCollection {
                {"api_dev_key", this.pastebinApiDevKey},
                {"api_option", "paste"},
                {"api_paste_code", sb.ToString()}
            };

            var url = this.requestsService.PostData(this.loginUrl, parameters);
            return url;
        }
    }
}