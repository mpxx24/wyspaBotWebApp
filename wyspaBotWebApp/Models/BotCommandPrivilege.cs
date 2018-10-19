namespace wyspaBotWebApp.Models {
    public class BotCommandPrivilege {
        public virtual string CommandId { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual bool IsAvailable { get; set; }
    }
}