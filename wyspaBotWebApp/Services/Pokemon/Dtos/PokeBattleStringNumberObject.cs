namespace wyspaBotWebApp.Services.Pokemon.Dtos {
    public class PokeBattleStringNumberObject {
        public int Value { get; set; }
        public string Name { get; set; }

        public PokeBattleStringNumberObject(int value, string name) {
            this.Value = value;
            this.Name = name;
        }
    }
}