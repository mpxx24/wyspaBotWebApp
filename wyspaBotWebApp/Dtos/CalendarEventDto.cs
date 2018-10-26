using System;

namespace wyspaBotWebApp.Dtos {
    public class CalendarEventDto {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
        public DateTime When { get; set; }
        public DateTime Added { get; set; }
        public string AddedBy { get; set; }
    }
}