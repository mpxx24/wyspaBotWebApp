using System.Collections.Specialized;

namespace wyspaBotWebApp.Services {
    public interface IRequestsService {
        string GetData(string address);
        string PostData(string address, string parameters);
        string PostData(string address, NameValueCollection parameters);
    }
}