using AwesomeDevEvents.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Persistence;

public class DevEventsDBContext: DbContext
{
    public DbSet<DevEvent> Events { get; set; }
    public DbSet<DevEventSpeaker> EventsSpeakers { get; set; }

    public DevEventsDBContext(DbContextOptions<DevEventsDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DevEvent>(e =>
        {
            e.HasKey(de => de.Id);
            e.Property(de => de.Title).IsRequired(false);
            e.Property(de => de.Description)
                .HasMaxLength(200)
                .HasColumnType("varchar(200)");
            e.HasMany(de => de.Speakers)
                .WithOne()
                .HasForeignKey(s => s.DevEventId);
        });

        modelBuilder.Entity<DevEventSpeaker>(e =>
        {
            e.HasKey(de => de.Id);
        });
    }
}
