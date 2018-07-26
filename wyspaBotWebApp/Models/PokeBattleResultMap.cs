using FluentNHibernate.Mapping;

namespace wyspaBotWebApp.Models {
    public class PokeBattleResultMap : ClassMap<PokeBattleResult> {
        public PokeBattleResultMap() {
            this.Table("WatchedProcesses");
            this.Id(x => x.Id).Column("uId").GeneratedBy.Increment();
            this.Map(x => x.PlayerNick).Column("vPlayerName");
            this.Map(x => x.OpponentNick).Column("vOpponentNick");
            this.Map(x => x.WinnerNick).Column("vWinnerNick");
            this.Map(x => x.WinningPokemonName).Column("vWinningPokemonName");
            this.Map(x => x.LoosingPokemonName).Column("vLoosingPokemonName");
        }
    }
}