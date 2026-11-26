namespace MyHybridApp2.Contracts.ClassRecords;

/// <summary>Student attendance and diary data for a section on a given date.</summary>
public sealed record SectionRecordsResponse(
    string SchoolId,
    string SectionId,
    string SectionName,
    DateOnly RecordDate,
    IReadOnlyList<SectionStudentRecordResponse> Students);

/// <summary>A student and their attendance and diary data for one date.</summary>
public sealed record SectionStudentRecordResponse(
    string StudentId,
    string Name,
    string StudentNumber,
    string Initials,
    bool IsPresent,
    string DiaryEntry);

/// <summary>Attendance status submitted for a student.</summary>
public sealed record StudentAttendanceUpdate(string StudentId, bool IsPresent);

/// <summary>Attendance updates for every student in a section on one date.</summary>
public sealed record SaveSectionAttendanceRequest(
    DateOnly RecordDate,
    IReadOnlyList<StudentAttendanceUpdate> Students);

/// <summary>A diary note submitted for one or more students.</summary>
public sealed record SaveSectionDiaryRequest(
    DateOnly RecordDate,
    IReadOnlyList<string> StudentIds,
    string Entry);

/// <summary>Summary returned after saving section records.</summary>
public sealed record SectionRecordSaveResponse(DateOnly RecordDate, int UpdatedCount);
