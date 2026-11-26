using MyHybridApp2.Application.ClassRecords;
using MyHybridApp2.Contracts.ClassRecords;

namespace MyHybridApp2.Api.Endpoints;

internal static class SectionRecordEndpoints
{
    private const string DefaultSchoolId = "oakridge-school";

    public static IEndpointRouteBuilder MapSectionRecordEndpoints(this IEndpointRouteBuilder app)
    {
        var sections = app.MapGroup("/api/sections/{sectionId}/records").WithTags("Section Records");

        sections.MapGet("/", (
                string sectionId,
                DateOnly date,
                ISectionRecordService service,
                CancellationToken ct,
                string? schoolId = null) =>
                service.GetAsync(schoolId ?? DefaultSchoolId, sectionId, date, ct))
            .WithName("GetSectionRecords")
            .WithSummary("Get attendance and diary entries for a section")
            .WithDescription("Returns the demo roster and saved records for the requested date.")
            .Produces<SectionRecordsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        sections.MapPut("/attendance", async (
                string sectionId,
                SaveSectionAttendanceRequest request,
                ISectionRecordService service,
                CancellationToken ct,
                string? schoolId = null) =>
                TypedResults.Ok(await service.SaveAttendanceAsync(
                    schoolId ?? DefaultSchoolId,
                    sectionId,
                    request,
                    ct)))
            .WithName("SaveSectionAttendance")
            .WithSummary("Save attendance for a section")
            .WithDescription("Saves the attendance state for every student in the section on the requested date.")
            .Produces<SectionRecordSaveResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        sections.MapPut("/diary", async (
                string sectionId,
                SaveSectionDiaryRequest request,
                ISectionRecordService service,
                CancellationToken ct,
                string? schoolId = null) =>
                TypedResults.Ok(await service.SaveDiaryAsync(
                    schoolId ?? DefaultSchoolId,
                    sectionId,
                    request,
                    ct)))
            .WithName("SaveSectionDiary")
            .WithSummary("Save a diary note for selected students")
            .WithDescription("Saves the note for each selected student on the requested date.")
            .Produces<SectionRecordSaveResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
