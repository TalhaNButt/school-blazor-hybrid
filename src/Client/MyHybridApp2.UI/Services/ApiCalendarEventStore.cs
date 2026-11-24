using System.Net.Http.Json;
using System.Text.Json;
using MyHybridApp2.Contracts.Calendar;
using MyHybridApp2.UI.Models;

namespace MyHybridApp2.UI.Services;

public sealed class ApiCalendarEventStore(HttpClient httpClient) : ICalendarEventStore
{
    private const string EventsPath = "api/calendar-events";

    public Task<IReadOnlyList<CalendarEvent>> GetAllAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync<IReadOnlyList<CalendarEvent>>(async () =>
        {
            var events = await httpClient.GetFromJsonAsync<List<CalendarEventResponse>>(
                EventsPath,
                cancellationToken);
            return events?.Select(ToModel).ToList()
                ?? throw new JsonException("The calendar event response was empty.");
        }, "Calendar events could not be loaded from the server.");

    public Task<CalendarEvent> CreateAsync(
        CalendarEvent calendarEvent,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            using var response = await httpClient.PostAsJsonAsync(
                EventsPath,
                ToCreateRequest(calendarEvent),
                cancellationToken);
            response.EnsureSuccessStatusCode();
            return ToModel(await ReadResponseAsync<CalendarEventResponse>(response, cancellationToken));
        }, "The calendar event could not be created on the server.");

    public Task<CalendarEvent> UpdateAsync(
        CalendarEvent calendarEvent,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            using var response = await httpClient.PutAsJsonAsync(
                $"{EventsPath}/{calendarEvent.Id}",
                ToUpdateRequest(calendarEvent),
                cancellationToken);
            response.EnsureSuccessStatusCode();
            return ToModel(await ReadResponseAsync<CalendarEventResponse>(response, cancellationToken));
        }, "The calendar event could not be updated on the server.");

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(async () =>
        {
            using var response = await httpClient.DeleteAsync($"{EventsPath}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return true;
        }, "The calendar event could not be deleted from the server.");
    }

    private static CreateCalendarEventRequest ToCreateRequest(CalendarEvent calendarEvent) => new(
        calendarEvent.Title,
        calendarEvent.Category,
        calendarEvent.Location,
        calendarEvent.Description,
        calendarEvent.StartsAt,
        calendarEvent.EndsAt,
        calendarEvent.ReminderMinutesBefore);

    private static UpdateCalendarEventRequest ToUpdateRequest(CalendarEvent calendarEvent) => new(
        calendarEvent.Title,
        calendarEvent.Category,
        calendarEvent.Location,
        calendarEvent.Description,
        calendarEvent.StartsAt,
        calendarEvent.EndsAt,
        calendarEvent.ReminderMinutesBefore);

    private static CalendarEvent ToModel(CalendarEventResponse response) => new()
    {
        Id = response.Id,
        Title = response.Title,
        Category = response.Category,
        Location = response.Location,
        Description = response.Description,
        StartsAt = response.StartsAt.ToLocalTime(),
        EndsAt = response.EndsAt.ToLocalTime(),
        ReminderMinutesBefore = response.ReminderMinutesBefore
    };

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken) =>
        await response.Content.ReadFromJsonAsync<T>(cancellationToken)
        ?? throw new JsonException("The calendar event response was empty.");

    private async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, string errorMessage)
    {
        try
        {
            return await operation();
        }
        catch (HttpRequestException exception)
        {
            var detail = exception.StatusCode is { } statusCode
                ? $"The server returned HTTP {(int)statusCode} ({statusCode})."
                : $"Could not connect to the API at {httpClient.BaseAddress}.";
            throw new InvalidOperationException($"{errorMessage} {detail}", exception);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException($"{errorMessage} The server response was invalid.", exception);
        }
    }
}
