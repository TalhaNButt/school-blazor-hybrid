using MyHybridApp2.Domain.Common;

namespace MyHybridApp2.Domain.Calendar;

public sealed class CalendarEvent : Entity
{
    public const int TitleMaxLength = 100;
    public const int CategoryMaxLength = 30;
    public const int LocationMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    private CalendarEvent() { }

    public string Title { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public int ReminderMinutesBefore { get; private set; }

    public static CalendarEvent Create(
        string title,
        string category,
        string location,
        string description,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        int reminderMinutesBefore)
    {
        var calendarEvent = new CalendarEvent();
        calendarEvent.Update(title, category, location, description, startsAt, endsAt, reminderMinutesBefore);
        return calendarEvent;
    }

    public void Update(
        string title,
        string category,
        string location,
        string description,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        int reminderMinutesBefore)
    {
        Title = title.Trim();
        Category = category;
        Location = location.Trim();
        Description = description.Trim();
        StartsAt = startsAt;
        EndsAt = endsAt;
        ReminderMinutesBefore = reminderMinutesBefore;
    }
}
