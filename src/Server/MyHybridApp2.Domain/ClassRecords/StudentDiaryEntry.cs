using MyHybridApp2.Domain.Common;

namespace MyHybridApp2.Domain.ClassRecords;

public sealed class StudentDiaryEntry : Entity
{
    public const int SchoolIdMaxLength = 100;
    public const int SectionIdMaxLength = 100;
    public const int StudentIdMaxLength = 100;
    public const int EntryMaxLength = 500;

    private StudentDiaryEntry() { }

    public string SchoolId { get; private set; } = string.Empty;
    public string SectionId { get; private set; } = string.Empty;
    public string StudentId { get; private set; } = string.Empty;
    public DateOnly RecordDate { get; private set; }
    public string Entry { get; private set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; private set; }

    public static StudentDiaryEntry Create(
        string schoolId,
        string sectionId,
        string studentId,
        DateOnly recordDate,
        string entry,
        DateTimeOffset updatedAt) =>
        new()
        {
            SchoolId = schoolId,
            SectionId = sectionId,
            StudentId = studentId,
            RecordDate = recordDate,
            Entry = entry.Trim(),
            UpdatedAt = updatedAt
        };

    public void Update(string entry, DateTimeOffset updatedAt)
    {
        Entry = entry.Trim();
        UpdatedAt = updatedAt;
    }
}
