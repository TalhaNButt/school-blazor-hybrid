using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHybridApp2.Domain.ClassRecords;

namespace MyHybridApp2.Infrastructure.Persistence.Configurations;

internal sealed class StudentDiaryEntryConfiguration : IEntityTypeConfiguration<StudentDiaryEntry>
{
    public void Configure(EntityTypeBuilder<StudentDiaryEntry> builder)
    {
        builder.ToTable("student_diary_entries");
        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(entry => entry.SchoolId).HasMaxLength(StudentDiaryEntry.SchoolIdMaxLength).IsRequired();
        builder.Property(entry => entry.SectionId).HasMaxLength(StudentDiaryEntry.SectionIdMaxLength).IsRequired();
        builder.Property(entry => entry.StudentId).HasMaxLength(StudentDiaryEntry.StudentIdMaxLength).IsRequired();
        builder.Property(entry => entry.RecordDate).IsRequired();
        builder.Property(entry => entry.Entry).HasMaxLength(StudentDiaryEntry.EntryMaxLength).IsRequired();
        builder.Property(entry => entry.UpdatedAt).IsRequired();
        builder.HasIndex(entry => new { entry.SchoolId, entry.SectionId, entry.StudentId, entry.RecordDate }).IsUnique();
        builder.HasIndex(entry => new { entry.SchoolId, entry.SectionId, entry.RecordDate });
    }
}
