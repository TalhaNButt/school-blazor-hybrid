using MyHybridApp2.UI.Models;

namespace MyHybridApp2.UI.Services;

public interface ICalendarEventStore
{
    Task<IReadOnlyList<CalendarEvent>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CalendarEvent> CreateAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default);
    Task<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
