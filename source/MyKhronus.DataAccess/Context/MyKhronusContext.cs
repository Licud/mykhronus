namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;

using MyKhronus.DataAccess.Entities;
using MyKhronus.DataAccess.Extensions;

internal class MyKhronusContext : DbContext
{
    public MyKhronusContext(DbContextOptions<MyKhronusContext> options)
    : base(options)
    {
    }

    public DbSet<Activity> Activities { get; set; }

    public DbSet<ActivityRecord> ActivityRecords { get; set; }

    public DbSet<WorkItem> WorkItems { get; set; }

    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RemovePluralizingNameConventions();

        base.OnModelCreating(modelBuilder);
    }
}
