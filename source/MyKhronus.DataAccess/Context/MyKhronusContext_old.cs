namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;

using MyKhronus.DataAccess.Extensions;
using MyKhronus.Models;

public class MyKhronusContext_old : DbContext
{
    public MyKhronusContext_old(DbContextOptions<MyKhronusContext_old> options)
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
