using FluentNHibernate.Mapping;

namespace wyspaBotWebApp.Models {
    public class BotCommandPrivilegeMap : ClassMap<BotCommandPrivilege> {
        public BotCommandPrivilegeMap() {
            this.Table("WatchedProcesses");
            this.Id(x => x.CommandId).Column("uId");
            this.Map(x => x.DisplayName).Column("vDisplayName");
            this.Map(x => x.IsAvailable).Column("bIsAvailable");
        }
    }
}