using System.Net;
using System.Net.Http.Json;
using MyHybridApp2.Contracts.Calendar;

namespace MyHybridApp2.Api.IntegrationTests;

public sealed class CalendarEventEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CalendarEvent_CreateGetUpdateDelete_PersistsChanges()
    {
        var ct = TestContext.Current.CancellationToken;
        var startsAt = DateTimeOffset.UtcNow.AddDays(1);
        var create = new CreateCalendarEventRequest(
            "Science exam",
            "Exam",
            "Room 204",
            "Review chapters 1-5",
            startsAt,
            startsAt.AddHours(1),
            15);

        var createResponse = await _client.PostAsJsonAsync("/api/calendar-events", create, ct);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var created = await createResponse.Content.ReadFromJsonAsync<CalendarEventResponse>(ct);
        Assert.NotNull(created);

        var fetched = await _client.GetFromJsonAsync<CalendarEventResponse>(
            $"/api/calendar-events/{created.Id}",
            ct);
        Assert.Equal(created, fetched);

        var update = new UpdateCalendarEventRequest(
            "Science exam - updated",
            "Exam",
            "Room 205",
            "Bring a calculator",
            startsAt.AddDays(1),
            startsAt.AddDays(1).AddHours(1),
            30);
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/calendar-events/{created.Id}",
            update,
            ct);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<CalendarEventResponse>(ct);
        Assert.NotNull(updated);
        Assert.Equal(update.Title, updated.Title);
        Assert.Equal(update.StartsAt, updated.StartsAt);
        Assert.Equal(update.ReminderMinutesBefore, updated.ReminderMinutesBefore);

        var events = await _client.GetFromJsonAsync<List<CalendarEventResponse>>("/api/calendar-events", ct);
        Assert.Contains(updated, events!);

        var deleteResponse = await _client.DeleteAsync($"/api/calendar-events/{created.Id}", ct);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingResponse = await _client.GetAsync($"/api/calendar-events/{created.Id}", ct);
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }

    [Fact]
    public async Task CalendarEvent_CreateInvalidRequest_ReturnsValidationProblem()
    {
        var startsAt = DateTimeOffset.UtcNow.AddDays(1);
        var request = new CreateCalendarEventRequest(
            "",
            "Unknown",
            "",
            "",
            startsAt,
            startsAt,
            -1);

        var response = await _client.PostAsJsonAsync(
            "/api/calendar-events",
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
