using System.Collections.Generic;
using System.Linq;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Dtos.Configuration;
using wyspaBotWebApp.Models;

namespace wyspaBotWebApp.Services.Configuration {
    public class BotConfigurationService : IBotConfigurationService {
        private readonly Repository<BotCommandPrivilege> botCommandPrivilegeRepository;

        public BotConfigurationService(Repository<BotCommandPrivilege> botCommandPrivilegeRepository) {
            this.botCommandPrivilegeRepository = botCommandPrivilegeRepository;
        }

        public IEnumerable<BotCommandPrivilegeDto> GetCommandsConfiguration() {
            var commands = this.botCommandPrivilegeRepository.GetAll();

            return commands.Select(x => new BotCommandPrivilegeDto {CommandId = x.CommandId, DisplayName = x.DisplayName, IsAvailable = x.IsAvailable});
        }
    }
}