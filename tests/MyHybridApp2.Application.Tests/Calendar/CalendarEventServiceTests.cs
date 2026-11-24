using FluentValidation;
using MyHybridApp2.Application.Calendar;
using MyHybridApp2.Application.Common;
using MyHybridApp2.Contracts.Calendar;

namespace MyHybridApp2.Application.Tests.Calendar;

public sealed class CalendarEventServiceTests
{
    [Fact]
    public async Task CreateUpdateAndDelete_PersistsCalendarEvent()
    {
        await using var db = TestDbContext.Create();
        var service = CreateService(db);
        var startsAt = new DateTimeOffset(2026, 12, 1, 9, 0, 0, TimeSpan.FromHours(5));
        var createRequest = new CreateCalendarEventRequest(
            "Science exam",
            "Exam",
            "Room 204",
            "Review chapters 1-5",
            startsAt,
            startsAt.AddHours(1),
            15);

        var created = await service.CreateAsync(createRequest, TestContext.Current.CancellationToken);
        var fetched = await service.GetByIdAsync(created.Id, TestContext.Current.CancellationToken);

        Assert.Equal(createRequest.Title, fetched.Title);
        Assert.Equal(createRequest.StartsAt.ToUniversalTime(), fetched.StartsAt);

        var updateRequest = new UpdateCalendarEventRequest(
            "Science exam - updated",
            "Exam",
            "Room 205",
            "Bring a calculator",
            startsAt.AddDays(1),
            startsAt.AddDays(1).AddHours(1),
            30);
        var updated = await service.UpdateAsync(
            created.Id,
            updateRequest,
            TestContext.Current.CancellationToken);

        Assert.Equal(updateRequest.Title, updated.Title);
        Assert.Equal(updateRequest.StartsAt.ToUniversalTime(), updated.StartsAt);
        Assert.Equal(updateRequest.ReminderMinutesBefore, updated.ReminderMinutesBefore);
        Assert.Contains(updated, await service.GetAllAsync(TestContext.Current.CancellationToken));

        await service.DeleteAsync(created.Id, TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetByIdAsync(created.Id, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateInvalidCalendarEvent_ThrowsValidationException()
    {
        await using var db = TestDbContext.Create();
        var service = CreateService(db);
        var startsAt = DateTimeOffset.UtcNow.AddDays(1);
        var request = new CreateCalendarEventRequest(
            "",
            "Unknown",
            "",
            "",
            startsAt,
            startsAt,
            -1);

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateAsync(request, TestContext.Current.CancellationToken));
    }

    private static CalendarEventService CreateService(TestDbContext db) =>
        new(db, new CreateCalendarEventRequestValidator(), new UpdateCalendarEventRequestValidator());
}
