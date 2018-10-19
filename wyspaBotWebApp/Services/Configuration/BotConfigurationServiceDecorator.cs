using System.Collections.Generic;
using System.Linq;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Dtos.Configuration;
using wyspaBotWebApp.Models;

namespace wyspaBotWebApp.Services.Configuration {
    public class BotConfigurationServiceDecorator : IBotConfigurationService {
        private readonly Repository<BotCommandPrivilege> botCommandPrivilegeRepository;

        private readonly IBotConfigurationService decorated;

        private List<BotCommandPrivilegeDto> privilegesCache = new List<BotCommandPrivilegeDto>();

        private bool isCacheValid;

        public BotConfigurationServiceDecorator(IBotConfigurationService decorated, Repository<BotCommandPrivilege> botCommandPrivilegeRepository) {
            this.decorated = decorated;
            this.botCommandPrivilegeRepository = botCommandPrivilegeRepository;
        }

        public IEnumerable<BotCommandPrivilegeDto> GetCommandsConfiguration() {
            if (this.isCacheValid) {
                return this.privilegesCache;
            }
            return this.decorated.GetCommandsConfiguration();
        }

        private void RefreshCacheIfNeeded() {
            var commandPrivileges = this.botCommandPrivilegeRepository.GetAll();
            this.privilegesCache = commandPrivileges.Select(x => new BotCommandPrivilegeDto {
                CommandId = x.CommandId,
                DisplayName = x.DisplayName,
                IsAvailable = x.IsAvailable
            }).ToList();

            this.isCacheValid = true;
        }
    }
}