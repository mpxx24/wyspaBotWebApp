namespace wyspaBotWebApp.Models {
    public class PokeBattleResult {
        public virtual int Id { get; set; }

        public virtual string PlayerNick { get; set; }

        public virtual string OpponentNick { get; set; }

        public virtual string WinnerNick { get; set; }

        public virtual string WinningPokemonName { get; set; }

        public virtual string LoosingPokemonName { get; set; }
    }
}