using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHybridApp2.Domain.ClassRecords;

namespace MyHybridApp2.Infrastructure.Persistence.Configurations;

internal sealed class StudentAttendanceConfiguration : IEntityTypeConfiguration<StudentAttendance>
{
    public void Configure(EntityTypeBuilder<StudentAttendance> builder)
    {
        builder.ToTable("student_attendance");
        builder.HasKey(attendance => attendance.Id);
        builder.Property(attendance => attendance.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(attendance => attendance.SchoolId).HasMaxLength(StudentAttendance.SchoolIdMaxLength).IsRequired();
        builder.Property(attendance => attendance.SectionId).HasMaxLength(StudentAttendance.SectionIdMaxLength).IsRequired();
        builder.Property(attendance => attendance.StudentId).HasMaxLength(StudentAttendance.StudentIdMaxLength).IsRequired();
        builder.Property(attendance => attendance.RecordDate).IsRequired();
        builder.Property(attendance => attendance.IsPresent).IsRequired();
        builder.Property(attendance => attendance.UpdatedAt).IsRequired();
        builder.HasIndex(attendance => new { attendance.SchoolId, attendance.SectionId, attendance.StudentId, attendance.RecordDate }).IsUnique();
        builder.HasIndex(attendance => new { attendance.SchoolId, attendance.SectionId, attendance.RecordDate });
    }
}
