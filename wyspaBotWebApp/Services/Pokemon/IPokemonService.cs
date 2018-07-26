using System.Collections.Generic;

namespace wyspaBotWebApp.Services.Pokemon {
    public interface IPokemonService {
        IEnumerable<string> PerformBattle(string challengerName, string opponentName);
        IEnumerable<string> GetPokeBattleStats();
        void ClearStats();
    }
}