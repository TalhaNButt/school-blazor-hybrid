using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Application.ClassRecords;
using MyHybridApp2.Application.Common;
using MyHybridApp2.Contracts.ClassRecords;

namespace MyHybridApp2.Application.Tests.ClassRecords;

public sealed class SectionRecordServiceTests
{
    [Fact]
    public async Task GetRecords_ReturnsDemoRosterAndSavedAttendanceAndDiary()
    {
        await using var db = TestDbContext.Create();
        var service = CreateService(db);
        var date = new DateOnly(2026, 11, 26);
        var initial = await service.GetAsync("oakridge-school", "grade-10-a", date, TestContext.Current.CancellationToken);

        Assert.Equal("oakridge-school", initial.SchoolId);
        Assert.Equal(8, initial.Students.Count);
        Assert.All(initial.Students, student => Assert.False(student.IsPresent));

        var attendance = initial.Students
            .Select((student, index) => new StudentAttendanceUpdate(student.StudentId, index < 5))
            .ToList();
        await service.SaveAttendanceAsync(
            "oakridge-school",
            "grade-10-a",
            new SaveSectionAttendanceRequest(date, attendance),
            TestContext.Current.CancellationToken);
        await service.SaveAttendanceAsync(
            "oakridge-school",
            "grade-10-a",
            new SaveSectionAttendanceRequest(date, attendance),
            TestContext.Current.CancellationToken);

        await service.SaveDiaryAsync(
            "oakridge-school",
            "grade-10-a",
            new SaveSectionDiaryRequest(date, ["student-001", "student-002"], "  Great work today.  "),
            TestContext.Current.CancellationToken);

        var saved = await service.GetAsync("oakridge-school", "grade-10-a", date, TestContext.Current.CancellationToken);

        Assert.Equal(5, saved.Students.Count(student => student.IsPresent));
        Assert.Equal("Great work today.", Assert.Single(saved.Students, student => student.StudentId == "student-001").DiaryEntry);
        Assert.Equal("Great work today.", Assert.Single(saved.Students, student => student.StudentId == "student-002").DiaryEntry);
        Assert.Empty(Assert.Single(saved.Students, student => student.StudentId == "student-003").DiaryEntry);
        Assert.Equal(8, await db.StudentAttendances.CountAsync(TestContext.Current.CancellationToken));
        Assert.Equal(2, await db.StudentDiaryEntries.CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SaveAttendance_RequiresEveryDemoStudent()
    {
        await using var db = TestDbContext.Create();
        var service = CreateService(db);

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.SaveAttendanceAsync(
                "oakridge-school",
                "grade-10-a",
                new SaveSectionAttendanceRequest(
                    new DateOnly(2026, 11, 26),
                    [new StudentAttendanceUpdate("student-001", true)]),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetUnknownSection_ThrowsNotFound()
    {
        await using var db = TestDbContext.Create();
        var service = CreateService(db);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetAsync(
                "oakridge-school",
                "unknown-section",
                new DateOnly(2026, 11, 26),
                TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData("greenwood-high", "grade-9-a")]
    [InlineData("greenwood-high", "grade-9-b")]
    [InlineData("greenwood-high", "grade-10-a")]
    [InlineData("riverside-academy", "grade-7-a")]
    [InlineData("riverside-academy", "grade-7-b")]
    [InlineData("riverside-academy", "grade-8-a")]
    [InlineData("oakridge-school", "grade-10-a")]
    [InlineData("oakridge-school", "grade-10-b")]
    [InlineData("oakridge-school", "grade-11-a")]
    public async Task GetRecords_ReturnsDemoRosterForEachSchoolSection(string schoolId, string sectionId)
    {
        await using var db = TestDbContext.Create();
        var service = CreateService(db);

        var records = await service.GetAsync(
            schoolId,
            sectionId,
            new DateOnly(2026, 11, 26),
            TestContext.Current.CancellationToken);

        Assert.Equal(schoolId, records.SchoolId);
        Assert.Equal(sectionId, records.SectionId);
        Assert.Equal(8, records.Students.Count);
    }

    [Fact]
    public async Task SaveRecordsForSameSectionId_DoNotCrossSchools()
    {
        await using var db = TestDbContext.Create();
        var service = CreateService(db);
        var date = new DateOnly(2026, 11, 26);
        var oakridge = await service.GetAsync(
            "oakridge-school",
            "grade-10-a",
            date,
            TestContext.Current.CancellationToken);
        var attendance = oakridge.Students
            .Select(student => new StudentAttendanceUpdate(student.StudentId, true))
            .ToList();

        await service.SaveAttendanceAsync(
            "oakridge-school",
            "grade-10-a",
            new SaveSectionAttendanceRequest(date, attendance),
            TestContext.Current.CancellationToken);
        await service.SaveDiaryAsync(
            "oakridge-school",
            "grade-10-a",
            new SaveSectionDiaryRequest(date, ["student-001"], "Oakridge note."),
            TestContext.Current.CancellationToken);

        var greenwood = await service.GetAsync(
            "greenwood-high",
            "grade-10-a",
            date,
            TestContext.Current.CancellationToken);

        Assert.All(greenwood.Students, student => Assert.False(student.IsPresent));
        Assert.Empty(Assert.Single(greenwood.Students, student => student.StudentId == "student-001").DiaryEntry);
        Assert.Equal(8, await db.StudentAttendances.CountAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, await db.StudentDiaryEntries.CountAsync(TestContext.Current.CancellationToken));
    }

    private static SectionRecordService CreateService(TestDbContext db) =>
        new(
            db,
            new SaveSectionAttendanceRequestValidator(),
            new SaveSectionDiaryRequestValidator(),
            TimeProvider.System);
}
