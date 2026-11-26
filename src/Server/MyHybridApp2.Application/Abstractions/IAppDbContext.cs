using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Domain.Calendar;
using MyHybridApp2.Domain.ClassRecords;
using MyHybridApp2.Domain.Posts;

namespace MyHybridApp2.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<CalendarEvent> CalendarEvents { get; }
    DbSet<StudentAttendance> StudentAttendances { get; }
    DbSet<StudentDiaryEntry> StudentDiaryEntries { get; }
    DbSet<Post> Posts { get; }
    DbSet<PostComment> PostComments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
