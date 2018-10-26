using System.Collections.Generic;
using wyspaBotWebApp.Dtos;

namespace wyspaBotWebApp.Services.Calendar {
    public interface ICalendarService {
        void AddEntry(CalendarEventDto dto);

        IEnumerable<CalendarEventDto> GetAllEntries();

        CalendarEventDto GetNextEntry();
    }
}