namespace MyHybridApp2.UI.Models;

public sealed class CalendarEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = "Class";
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public int ReminderMinutesBefore { get; set; }
}
