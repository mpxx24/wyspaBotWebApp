using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace wyspaBotWebApp.Services.PasteBin {
    public class PasteBinApiService : IPasteBinApiService {
        private readonly string loginUrl = "http://pastebin.com/api/api_post.php";
        private readonly string pastebinApiDevKey;
        private readonly IRequestsService requestsService;

        public PasteBinApiService(string pastebinApiDevKey, IRequestsService requestsService) {
            this.pastebinApiDevKey = pastebinApiDevKey;
            this.requestsService = requestsService;
        }

        public string Save(IEnumerable<string> messages, string name = "history") {
            var sb = new StringBuilder();

            foreach (var message in messages) {
                sb.AppendLine(message);
            }

            var parameters = new NameValueCollection {
                {"api_dev_key", this.pastebinApiDevKey},
                {"api_option", "paste"},
                {"api_paste_name", name},
                {"api_paste_code", sb.ToString()}
            };

            var url = this.requestsService.PostData(this.loginUrl, parameters);
            return url;
        }

        public string Save(StringBuilder stringBuilder, string name = "history") {
            var parameters = new NameValueCollection {
                {"api_dev_key", this.pastebinApiDevKey},
                {"api_option", "paste"},
                {"api_paste_name", name},
                {"api_paste_code", stringBuilder.ToString()}
            };

            var url = this.requestsService.PostData(this.loginUrl, parameters);
            return url;
        }

        public string Save(string message, string name = "history") {
            var parameters = new NameValueCollection {
                {"api_dev_key", this.pastebinApiDevKey},
                {"api_option", "paste"},
                {"api_paste_name", name},
                {"api_paste_code", message}
            };

            var url = this.requestsService.PostData(this.loginUrl, parameters);
            return url;
        }
    }
}