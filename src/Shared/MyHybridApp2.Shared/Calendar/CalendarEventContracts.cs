namespace MyHybridApp2.Contracts.Calendar;

/// <summary>Calendar event data returned by the API.</summary>
public sealed record CalendarEventResponse(
    Guid Id,
    string Title,
    string Category,
    string Location,
    string Description,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    int ReminderMinutesBefore);

/// <summary>Payload for creating a calendar event.</summary>
public sealed record CreateCalendarEventRequest(
    string Title,
    string Category,
    string Location,
    string Description,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    int ReminderMinutesBefore);

/// <summary>Payload for updating a calendar event.</summary>
public sealed record UpdateCalendarEventRequest(
    string Title,
    string Category,
    string Location,
    string Description,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    int ReminderMinutesBefore);
