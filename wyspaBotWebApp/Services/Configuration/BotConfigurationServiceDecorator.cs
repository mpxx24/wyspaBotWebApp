using System.Collections.Generic;
using System.Linq;
using wyspaBotWebApp.Dtos.Configuration;

namespace wyspaBotWebApp.Services.Configuration {
    public class BotConfigurationServiceDecorator : IBotConfigurationService {
        private readonly IBotConfigurationService decorated;

        private List<BotCommandPrivilegeDto> privilegesCache = new List<BotCommandPrivilegeDto>();

        private bool isCacheValid;

        public BotConfigurationServiceDecorator(IBotConfigurationService decorated) {
            this.decorated = decorated;
        }

        public IEnumerable<BotCommandPrivilegeDto> GetCommandsConfiguration() {
            if (!this.isCacheValid) {
                this.RefreshCacheIfNeeded();
            }

            return this.privilegesCache;
        }

        public void UpdatePrivilege(BotCommandPrivilegeDto dto) {
            this.decorated.UpdatePrivilege(dto);
            this.isCacheValid = false;
        }

        private void RefreshCacheIfNeeded() {
            this.privilegesCache = this.decorated.GetCommandsConfiguration().ToList();
            this.isCacheValid = true;
        }
    }
}