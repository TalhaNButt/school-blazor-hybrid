using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Application.Common;
using MyHybridApp2.Domain.Calendar;
using MyHybridApp2.Contracts.Calendar;

namespace MyHybridApp2.Application.Calendar;

public interface ICalendarEventService
{
    Task<IReadOnlyList<CalendarEventResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<CalendarEventResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CalendarEventResponse> CreateAsync(CreateCalendarEventRequest request, CancellationToken cancellationToken);
    Task<CalendarEventResponse> UpdateAsync(Guid id, UpdateCalendarEventRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class CalendarEventService(
    IAppDbContext db,
    IValidator<CreateCalendarEventRequest> createValidator,
    IValidator<UpdateCalendarEventRequest> updateValidator) : ICalendarEventService
{
    public async Task<IReadOnlyList<CalendarEventResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var calendarEvents = await db.CalendarEvents
            .AsNoTracking()
            .OrderBy(calendarEvent => calendarEvent.StartsAt)
            .ToListAsync(cancellationToken);

        return calendarEvents.Select(ToResponse).ToList();
    }

    public async Task<CalendarEventResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var calendarEvent = await db.CalendarEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(CalendarEvent), id);

        return ToResponse(calendarEvent);
    }

    public async Task<CalendarEventResponse> CreateAsync(
        CreateCalendarEventRequest request,
        CancellationToken cancellationToken)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var calendarEvent = CalendarEvent.Create(
            request.Title,
            request.Category,
            request.Location,
            request.Description,
            request.StartsAt.ToUniversalTime(),
            request.EndsAt.ToUniversalTime(),
            request.ReminderMinutesBefore);

        db.CalendarEvents.Add(calendarEvent);
        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(calendarEvent);
    }

    public async Task<CalendarEventResponse> UpdateAsync(
        Guid id,
        UpdateCalendarEventRequest request,
        CancellationToken cancellationToken)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var calendarEvent = await db.CalendarEvents
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(CalendarEvent), id);

        calendarEvent.Update(
            request.Title,
            request.Category,
            request.Location,
            request.Description,
            request.StartsAt.ToUniversalTime(),
            request.EndsAt.ToUniversalTime(),
            request.ReminderMinutesBefore);

        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(calendarEvent);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var calendarEvent = await db.CalendarEvents
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(CalendarEvent), id);

        db.CalendarEvents.Remove(calendarEvent);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static CalendarEventResponse ToResponse(CalendarEvent calendarEvent) => new(
        calendarEvent.Id,
        calendarEvent.Title,
        calendarEvent.Category,
        calendarEvent.Location,
        calendarEvent.Description,
        calendarEvent.StartsAt,
        calendarEvent.EndsAt,
        calendarEvent.ReminderMinutesBefore);
}

public sealed class CreateCalendarEventRequestValidator : AbstractValidator<CreateCalendarEventRequest>
{
    public CreateCalendarEventRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().MaximumLength(CalendarEvent.TitleMaxLength);
        RuleFor(request => request.Category).NotEmpty();
        RuleFor(request => request.Category).Must(IsSupportedCategory);
        RuleFor(request => request.Location).NotNull().MaximumLength(CalendarEvent.LocationMaxLength);
        RuleFor(request => request.Description).NotNull().MaximumLength(CalendarEvent.DescriptionMaxLength);
        RuleFor(request => request.EndsAt).GreaterThan(request => request.StartsAt);
        RuleFor(request => request.ReminderMinutesBefore).Must(IsSupportedReminder);
    }

    private static bool IsSupportedCategory(string category) =>
        category is "Class" or "Exam" or "Activity" or "Meeting";

    private static bool IsSupportedReminder(int minutesBefore) =>
        minutesBefore is 0 or 5 or 15 or 30 or 60 or 1440;
}

public sealed class UpdateCalendarEventRequestValidator : AbstractValidator<UpdateCalendarEventRequest>
{
    public UpdateCalendarEventRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().MaximumLength(CalendarEvent.TitleMaxLength);
        RuleFor(request => request.Category).NotEmpty();
        RuleFor(request => request.Category).Must(category =>
            category is "Class" or "Exam" or "Activity" or "Meeting");
        RuleFor(request => request.Location).NotNull().MaximumLength(CalendarEvent.LocationMaxLength);
        RuleFor(request => request.Description).NotNull().MaximumLength(CalendarEvent.DescriptionMaxLength);
        RuleFor(request => request.EndsAt).GreaterThan(request => request.StartsAt);
        RuleFor(request => request.ReminderMinutesBefore).Must(minutesBefore =>
            minutesBefore is 0 or 5 or 15 or 30 or 60 or 1440);
    }
}
