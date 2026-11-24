using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHybridApp2.Domain.Calendar;

namespace MyHybridApp2.Infrastructure.Persistence.Configurations;

internal sealed class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("calendar_events");
        builder.HasKey(calendarEvent => calendarEvent.Id);
        builder.Property(calendarEvent => calendarEvent.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(calendarEvent => calendarEvent.Title).HasMaxLength(CalendarEvent.TitleMaxLength).IsRequired();
        builder.Property(calendarEvent => calendarEvent.Category).HasMaxLength(CalendarEvent.CategoryMaxLength).IsRequired();
        builder.Property(calendarEvent => calendarEvent.Location).HasMaxLength(CalendarEvent.LocationMaxLength).IsRequired();
        builder.Property(calendarEvent => calendarEvent.Description).HasMaxLength(CalendarEvent.DescriptionMaxLength).IsRequired();
        builder.Property(calendarEvent => calendarEvent.StartsAt).IsRequired();
        builder.Property(calendarEvent => calendarEvent.EndsAt).IsRequired();
        builder.Property(calendarEvent => calendarEvent.ReminderMinutesBefore).IsRequired();
        builder.HasIndex(calendarEvent => calendarEvent.StartsAt);
    }
}
