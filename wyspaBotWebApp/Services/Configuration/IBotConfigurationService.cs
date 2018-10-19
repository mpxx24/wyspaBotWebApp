using System.Collections.Generic;
using wyspaBotWebApp.Dtos.Configuration;

namespace wyspaBotWebApp.Services.Configuration {
    public interface IBotConfigurationService {
        IEnumerable<BotCommandPrivilegeDto> GetCommandsConfiguration();
    }
}
            