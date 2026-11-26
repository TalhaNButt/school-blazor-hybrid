using MyHybridApp2.Domain.Common;

namespace MyHybridApp2.Domain.ClassRecords;

public sealed class StudentAttendance : Entity
{
    public const int SchoolIdMaxLength = 100;
    public const int SectionIdMaxLength = 100;
    public const int StudentIdMaxLength = 100;

    private StudentAttendance() { }

    public string SchoolId { get; private set; } = string.Empty;
    public string SectionId { get; private set; } = string.Empty;
    public string StudentId { get; private set; } = string.Empty;
    public DateOnly RecordDate { get; private set; }
    public bool IsPresent { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static StudentAttendance Create(
        string schoolId,
        string sectionId,
        string studentId,
        DateOnly recordDate,
        bool isPresent,
        DateTimeOffset updatedAt) =>
        new()
        {
            SchoolId = schoolId,
            SectionId = sectionId,
            StudentId = studentId,
            RecordDate = recordDate,
            IsPresent = isPresent,
            UpdatedAt = updatedAt
        };

    public void Update(bool isPresent, DateTimeOffset updatedAt)
    {
        IsPresent = isPresent;
        UpdatedAt = updatedAt;
    }
}
