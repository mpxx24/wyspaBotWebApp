using System.Collections.Generic;

namespace wyspaBotWebApp.Services {
    public interface IPasteBinApiService {
        string Save(IEnumerable<string> messages);
    }
}