using System.Collections.Generic;
using System.Text;

namespace wyspaBotWebApp.Services {
    public interface IPasteBinApiService {
        string Save(IEnumerable<string> messages, string name = "history");

        string Save(StringBuilder stringBuilder, string name = "history");

        string Save(string message, string name = "history");
    }
}