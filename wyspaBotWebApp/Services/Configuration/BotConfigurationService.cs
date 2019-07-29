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

        public void UpdatePrivilege(BotCommandPrivilegeDto dto) {
            var itemToUpdate = this.botCommandPrivilegeRepository.Get(dto.CommandId);
            itemToUpdate.DisplayName = dto.DisplayName;
            itemToUpdate.IsAvailable = dto.IsAvailable;

            this.botCommandPrivilegeRepository.Update(itemToUpdate);
        }
    }
}