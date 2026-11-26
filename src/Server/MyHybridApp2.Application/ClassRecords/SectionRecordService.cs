using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Application.Common;
using MyHybridApp2.Domain.ClassRecords;
using MyHybridApp2.Contracts.ClassRecords;

namespace MyHybridApp2.Application.ClassRecords;

public interface ISectionRecordService
{
    Task<SectionRecordsResponse> GetAsync(
        string schoolId,
        string sectionId,
        DateOnly recordDate,
        CancellationToken cancellationToken);
    Task<SectionRecordSaveResponse> SaveAttendanceAsync(
        string schoolId,
        string sectionId,
        SaveSectionAttendanceRequest request,
        CancellationToken cancellationToken);
    Task<SectionRecordSaveResponse> SaveDiaryAsync(
        string schoolId,
        string sectionId,
        SaveSectionDiaryRequest request,
        CancellationToken cancellationToken);
}

public sealed class SectionRecordService(
    IAppDbContext db,
    IValidator<SaveSectionAttendanceRequest> attendanceValidator,
    IValidator<SaveSectionDiaryRequest> diaryValidator,
    TimeProvider timeProvider) : ISectionRecordService
{
    private static readonly Dictionary<string, Dictionary<string, DemoSection>> Sections =
        new Dictionary<string, Dictionary<string, DemoSection>>(StringComparer.OrdinalIgnoreCase)
        {
            ["greenwood-high"] = CreateSections(
                "greenwood-high",
                ("grade-9-a", "Grade 9 · Section A"),
                ("grade-9-b", "Grade 9 · Section B"),
                ("grade-10-a", "Grade 10 · Section A")),
            ["riverside-academy"] = CreateSections(
                "riverside-academy",
                ("grade-7-a", "Grade 7 · Section A"),
                ("grade-7-b", "Grade 7 · Section B"),
                ("grade-8-a", "Grade 8 · Section A")),
            ["oakridge-school"] = CreateSections(
                "oakridge-school",
                ("grade-10-a", "Grade 10 · Section A"),
                ("grade-10-b", "Grade 10 · Section B"),
                ("grade-11-a", "Grade 11 · Section A"))
        };

    private static readonly IReadOnlyList<DemoStudent> Students =
    [
        new("student-001", "Aisha Rahman", "Student 001"),
        new("student-002", "Adam Khan", "Student 002"),
        new("student-003", "Fatima Noor", "Student 003"),
        new("student-004", "Hamza Ali", "Student 004"),
        new("student-005", "Ibrahim Malik", "Student 005"),
        new("student-006", "Maryam Ahmed", "Student 006"),
        new("student-007", "Musa Shah", "Student 007"),
        new("student-008", "Sara Iqbal", "Student 008")
    ];

    public async Task<SectionRecordsResponse> GetAsync(
        string schoolId,
        string sectionId,
        DateOnly recordDate,
        CancellationToken cancellationToken)
    {
        var section = GetSection(schoolId, sectionId);
        var attendance = await db.StudentAttendances
            .AsNoTracking()
            .Where(item =>
                item.SchoolId == section.SchoolId &&
                item.SectionId == section.Id &&
                item.RecordDate == recordDate)
            .ToDictionaryAsync(item => item.StudentId, item => item.IsPresent, cancellationToken);
        var diaryEntries = await db.StudentDiaryEntries
            .AsNoTracking()
            .Where(item =>
                item.SchoolId == section.SchoolId &&
                item.SectionId == section.Id &&
                item.RecordDate == recordDate)
            .ToDictionaryAsync(item => item.StudentId, item => item.Entry, cancellationToken);

        return new SectionRecordsResponse(
            section.SchoolId,
            section.Id,
            section.Name,
            recordDate,
            Students.Select(student => new SectionStudentRecordResponse(
                student.Id,
                student.Name,
                student.StudentNumber,
                student.Initials,
                attendance.GetValueOrDefault(student.Id),
                diaryEntries.GetValueOrDefault(student.Id, string.Empty)))
                .ToList());
    }

    public async Task<SectionRecordSaveResponse> SaveAttendanceAsync(
        string schoolId,
        string sectionId,
        SaveSectionAttendanceRequest request,
        CancellationToken cancellationToken)
    {
        await attendanceValidator.ValidateAndThrowAsync(request, cancellationToken);
        var section = GetSection(schoolId, sectionId);
        var updates = request.Students.ToDictionary(item => item.StudentId, item => item.IsPresent);
        var studentIds = Students.Select(student => student.Id).ToHashSet(StringComparer.Ordinal);
        if (updates.Count != Students.Count || updates.Keys.Any(studentId => !studentIds.Contains(studentId)))
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(request.Students), "Submit attendance for every student in this section.")
            ]);
        }

        var records = await db.StudentAttendances
            .Where(item =>
                item.SchoolId == section.SchoolId &&
                item.SectionId == section.Id &&
                item.RecordDate == request.RecordDate)
            .ToListAsync(cancellationToken);
        var existingByStudent = records.ToDictionary(item => item.StudentId, StringComparer.Ordinal);
        var updatedAt = timeProvider.GetUtcNow();

        foreach (var student in Students)
        {
            if (existingByStudent.TryGetValue(student.Id, out var attendance))
            {
                attendance.Update(updates[student.Id], updatedAt);
            }
            else
            {
                db.StudentAttendances.Add(StudentAttendance.Create(
                    section.SchoolId,
                    section.Id,
                    student.Id,
                    request.RecordDate,
                    updates[student.Id],
                    updatedAt));
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return new SectionRecordSaveResponse(request.RecordDate, Students.Count);
    }

    public async Task<SectionRecordSaveResponse> SaveDiaryAsync(
        string schoolId,
        string sectionId,
        SaveSectionDiaryRequest request,
        CancellationToken cancellationToken)
    {
        await diaryValidator.ValidateAndThrowAsync(request, cancellationToken);
        var section = GetSection(schoolId, sectionId);
        var studentIds = Students.Select(student => student.Id).ToHashSet(StringComparer.Ordinal);
        if (request.StudentIds.Any(studentId => !studentIds.Contains(studentId)))
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(request.StudentIds), "One or more selected students are not in this section.")
            ]);
        }

        var existingEntries = await db.StudentDiaryEntries
            .Where(item =>
                item.SectionId == section.Id &&
                item.SchoolId == section.SchoolId &&
                item.RecordDate == request.RecordDate &&
                request.StudentIds.Contains(item.StudentId))
            .ToListAsync(cancellationToken);
        var existingByStudent = existingEntries.ToDictionary(item => item.StudentId, StringComparer.Ordinal);
        var updatedAt = timeProvider.GetUtcNow();

        foreach (var studentId in request.StudentIds)
        {
            if (existingByStudent.TryGetValue(studentId, out var diaryEntry))
            {
                diaryEntry.Update(request.Entry, updatedAt);
            }
            else
            {
                db.StudentDiaryEntries.Add(StudentDiaryEntry.Create(
                    section.SchoolId,
                    section.Id,
                    studentId,
                    request.RecordDate,
                    request.Entry,
                    updatedAt));
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return new SectionRecordSaveResponse(request.RecordDate, request.StudentIds.Count);
    }

    private static Dictionary<string, DemoSection> CreateSections(
        string schoolId,
        params (string Id, string Name)[] sections) =>
        sections.ToDictionary(
            section => section.Id,
            section => new DemoSection(schoolId, section.Id, section.Name),
            StringComparer.OrdinalIgnoreCase);

    private static DemoSection GetSection(string schoolId, string sectionId) =>
        Sections.TryGetValue(schoolId, out var schoolSections) &&
        schoolSections.TryGetValue(sectionId, out var section)
            ? section
            : throw new NotFoundException("Section", $"{schoolId}/{sectionId}");

    private sealed record DemoSection(string SchoolId, string Id, string Name);

    private sealed record DemoStudent(string Id, string Name, string StudentNumber)
    {
        public string Initials { get; } = string.Join(
            string.Empty,
            Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(part => part[0]));
    }
}

public sealed class SaveSectionAttendanceRequestValidator : AbstractValidator<SaveSectionAttendanceRequest>
{
    public SaveSectionAttendanceRequestValidator()
    {
        RuleFor(request => request.RecordDate).NotEqual(default(DateOnly));
        RuleFor(request => request.Students)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(students => students.Select(student => student.StudentId).Distinct(StringComparer.Ordinal).Count() == students.Count)
            .WithMessage("A student can only appear once in the attendance list.");
        RuleForEach(request => request.Students).ChildRules(student =>
        {
            student.RuleFor(item => item.StudentId).NotEmpty().MaximumLength(StudentAttendance.StudentIdMaxLength);
        });
    }
}

public sealed class SaveSectionDiaryRequestValidator : AbstractValidator<SaveSectionDiaryRequest>
{
    public SaveSectionDiaryRequestValidator()
    {
        RuleFor(request => request.RecordDate).NotEqual(default(DateOnly));
        RuleFor(request => request.StudentIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(studentIds => studentIds.Distinct(StringComparer.Ordinal).Count() == studentIds.Count)
            .WithMessage("A student can only appear once in the diary list.");
        RuleForEach(request => request.StudentIds)
            .NotEmpty()
            .MaximumLength(StudentDiaryEntry.StudentIdMaxLength);
        RuleFor(request => request.Entry)
            .NotNull()
            .MaximumLength(StudentDiaryEntry.EntryMaxLength);
    }
}
