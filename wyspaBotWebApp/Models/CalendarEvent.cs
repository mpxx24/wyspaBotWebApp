using System;

namespace wyspaBotWebApp.Models {
    public class CalendarEvent {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Place { get; set; }
        public virtual DateTime When { get; set; }
        public virtual DateTime Added { get; set; }
        public virtual string AddedBy { get; set; }
    }
}