namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;

using MyKhronus.DataAccess.Entities;
using MyKhronus.DataAccess.Extensions;

internal class MyKhronusContext(DbContextOptions<MyKhronusContext> options) : DbContext(options)
{
    public DbSet<WorkItem> WorkItems { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<DayEntry> DayEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RemovePluralizingNameConventions();

        modelBuilder.Entity<DayEntry>()
            .HasKey(de => new { de.EntryDate, de.WorkItemId });

        base.OnModelCreating(modelBuilder);
    }
}
