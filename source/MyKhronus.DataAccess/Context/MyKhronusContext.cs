namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;

using MyKhronus.DataAccess.Entities;
using MyKhronus.DataAccess.Extensions;

internal class MyKhronusContext(DbContextOptions<MyKhronusContext> options) : DbContext(options)
{
    public DbSet<WorkItem> WorkItems { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<DayEntry> DayEntries { get; set; }

    public DbSet<ScheduledWorkItem> ScheduledWorkItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RemovePluralizingNameConventions();

        modelBuilder.Entity<DayEntry>()
            .HasKey(de => new { de.EntryDate, de.WorkItemId });

        modelBuilder.Entity<ScheduledWorkItem>(builder =>
        {
            builder.HasKey(s => s.WorkItemId);

            builder.HasOne(s => s.WorkItem)
                .WithMany()
                .HasForeignKey(s => s.WorkItemId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
