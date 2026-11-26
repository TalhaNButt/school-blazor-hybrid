using System.Net;
using System.Net.Http.Json;
using MyHybridApp2.Contracts.ClassRecords;

namespace MyHybridApp2.Api.IntegrationTests;

public sealed class SectionRecordEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task SectionRecords_SaveAndLoadAttendanceAndDiary()
    {
        var ct = TestContext.Current.CancellationToken;
        var date = new DateOnly(2026, 11, 26);
        const string sectionId = "grade-10-a";
        const string schoolId = "oakridge-school";
        var initial = await _client.GetFromJsonAsync<SectionRecordsResponse>(
            $"/api/sections/{sectionId}/records?date={date:yyyy-MM-dd}&schoolId={schoolId}",
            ct);

        Assert.NotNull(initial);
        Assert.Equal(schoolId, initial.SchoolId);
        Assert.Equal(8, initial.Students.Count);

        var attendance = initial.Students
            .Select((student, index) => new StudentAttendanceUpdate(student.StudentId, index < 6))
            .ToList();
        var attendanceResponse = await _client.PutAsJsonAsync(
            $"/api/sections/{sectionId}/records/attendance?schoolId={schoolId}",
            new SaveSectionAttendanceRequest(date, attendance),
            ct);

        Assert.Equal(HttpStatusCode.OK, attendanceResponse.StatusCode);

        var diaryResponse = await _client.PutAsJsonAsync(
            $"/api/sections/{sectionId}/records/diary?schoolId={schoolId}",
            new SaveSectionDiaryRequest(date, ["student-001", "student-004"], "A great day."),
            ct);
        Assert.Equal(HttpStatusCode.OK, diaryResponse.StatusCode);

        var saved = await _client.GetFromJsonAsync<SectionRecordsResponse>(
            $"/api/sections/{sectionId}/records?date={date:yyyy-MM-dd}&schoolId={schoolId}",
            ct);

        Assert.NotNull(saved);
        Assert.Equal(6, saved.Students.Count(student => student.IsPresent));
        Assert.Equal(
            "A great day.",
            Assert.Single(saved.Students, student => student.StudentId == "student-001").DiaryEntry);
        Assert.Empty(Assert.Single(saved.Students, student => student.StudentId == "student-002").DiaryEntry);
    }

    [Fact]
    public async Task SectionRecords_SupportsSectionsFromEachSchool()
    {
        var ct = TestContext.Current.CancellationToken;
        const string recordsDate = "2026-11-26";
        var greenwood = await _client.GetFromJsonAsync<SectionRecordsResponse>(
            $"/api/sections/grade-10-a/records?date={recordsDate}&schoolId=greenwood-high",
            ct);
        var oakridge = await _client.GetFromJsonAsync<SectionRecordsResponse>(
            $"/api/sections/grade-10-a/records?date={recordsDate}&schoolId=oakridge-school",
            ct);
        var riverside = await _client.GetFromJsonAsync<SectionRecordsResponse>(
            $"/api/sections/grade-7-a/records?date={recordsDate}&schoolId=riverside-academy",
            ct);

        Assert.Equal("greenwood-high", greenwood?.SchoolId);
        Assert.Equal("oakridge-school", oakridge?.SchoolId);
        Assert.Equal("riverside-academy", riverside?.SchoolId);
    }

    [Fact]
    public async Task SectionRecords_UnknownSection_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            "/api/sections/not-a-section/records?date=2026-11-26&schoolId=oakridge-school",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
