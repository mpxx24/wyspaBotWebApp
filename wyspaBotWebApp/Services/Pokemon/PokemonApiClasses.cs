using System.Collections.Generic;

namespace wyspaBotWebApp.Services.Pokemon {
    public class Ability2 {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Ability {
        public bool is_hidden { get; set; }
        public int slot { get; set; }
        public Ability2 ability { get; set; }
    }

    public class Stat2 {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Stat {
        public int base_stat { get; set; }
        public int effort { get; set; }
        public Stat2 stat { get; set; }
    }

    public class Type2 {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Type {
        public int slot { get; set; }
        public Type2 type { get; set; }
    }

    public class MoveLearnMethod {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class VersionGroupDetail {
        public int level_learned_at { get; set; }
        public VersionGroup version_group { get; set; }
        public MoveLearnMethod move_learn_method { get; set; }
    }

    public class Move {
        public Move2 move { get; set; }
        public List<VersionGroupDetail> version_group_details { get; set; }
    }

    public class Move2 {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class VersionGroup {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class PokemonApiRootObject {
        public int id { get; set; }
        public string name { get; set; }
        public List<Ability> abilities { get; set; }
        public List<Move> moves { get; set; }
        public List<Stat> stats { get; set; }
    }
}