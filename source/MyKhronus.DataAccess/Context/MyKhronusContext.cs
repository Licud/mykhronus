namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;

using MyKhronus.DataAccess.Extensions;
using MyKhronus.Models;

public class MyKhronusContext : DbContext
{
    public MyKhronusContext(DbContextOptions<MyKhronusContext> options)
        : base(options)
    {
    }

    public DbSet<Activity> Activities { get; set; }

    public DbSet<ActivityRecord> ActivityRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RemovePluralizingNameConventions();

        base.OnModelCreating(modelBuilder);
    }
}
