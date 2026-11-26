using MyHybridApp2.Contracts.ClassRecords;

namespace MyHybridApp2.UI.Services;

public interface ISectionRecordStore
{
    Task<SectionRecordsResponse> GetAsync(
        string schoolId,
        string sectionId,
        DateOnly recordDate,
        CancellationToken cancellationToken = default);
    Task SaveAttendanceAsync(
        string schoolId,
        string sectionId,
        SaveSectionAttendanceRequest request,
        CancellationToken cancellationToken = default);
    Task SaveDiaryAsync(
        string schoolId,
        string sectionId,
        SaveSectionDiaryRequest request,
        CancellationToken cancellationToken = default);
}
