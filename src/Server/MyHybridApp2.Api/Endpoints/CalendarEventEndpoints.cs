using MyHybridApp2.Application.Calendar;
using MyHybridApp2.Contracts.Calendar;

namespace MyHybridApp2.Api.Endpoints;

internal static class CalendarEventEndpoints
{
    public static IEndpointRouteBuilder MapCalendarEventEndpoints(this IEndpointRouteBuilder app)
    {
        var events = app.MapGroup("/api/calendar-events").WithTags("Calendar Events");

        events.MapGet("/", (ICalendarEventService service, CancellationToken ct) => service.GetAllAsync(ct))
            .WithName("GetCalendarEvents")
            .WithSummary("Get all calendar events")
            .WithDescription("Returns calendar events ordered by start time.")
            .Produces<IReadOnlyList<CalendarEventResponse>>(StatusCodes.Status200OK);

        events.MapGet("/{id:guid}",
                (Guid id, ICalendarEventService service, CancellationToken ct) => service.GetByIdAsync(id, ct))
            .WithName("GetCalendarEventById")
            .WithSummary("Get a calendar event by ID")
            .WithDescription("Returns one calendar event.")
            .Produces<CalendarEventResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        events.MapPost("/", async (
                CreateCalendarEventRequest request,
                ICalendarEventService service,
                CancellationToken ct) =>
            {
                var calendarEvent = await service.CreateAsync(request, ct);
                return TypedResults.CreatedAtRoute(calendarEvent, "GetCalendarEventById", new { id = calendarEvent.Id });
            })
            .WithName("CreateCalendarEvent")
            .WithSummary("Create a calendar event")
            .WithDescription("Creates an event and returns its resource URL.")
            .Produces<CalendarEventResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        events.MapPut("/{id:guid}", async (
                Guid id,
                UpdateCalendarEventRequest request,
                ICalendarEventService service,
                CancellationToken ct) =>
                TypedResults.Ok(await service.UpdateAsync(id, request, ct)))
            .WithName("UpdateCalendarEvent")
            .WithSummary("Update a calendar event")
            .WithDescription("Replaces the stored values for a calendar event.")
            .Produces<CalendarEventResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        events.MapDelete("/{id:guid}", async (
                Guid id,
                ICalendarEventService service,
                CancellationToken ct) =>
            {
                await service.DeleteAsync(id, ct);
                return TypedResults.NoContent();
            })
            .WithName("DeleteCalendarEvent")
            .WithSummary("Delete a calendar event")
            .WithDescription("Deletes a calendar event.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
