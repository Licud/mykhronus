namespace MyKhronus.DataAccess.DataUtility;

using Microsoft.EntityFrameworkCore;

using MyKhronus.DataAccess.Activities.Repositories;
using MyKhronus.DataAccess.ActivityRecords.Repositories;
using MyKhronus.DataAccess.Context;

internal class MyKhronusContextUnitOfWork(IDbContextFactory<MyKhronusContext> contextFactory) 
    : IUnitOfWork
{
    private MyKhronusContext context = contextFactory.CreateDbContext();
    private ActivityRepository activityRepository;
    private ActivityRecordRepository activityRecordRepository;

    private bool isDisposed = false;

    public IActivityRepository GetActivityRepository()
    {
        return activityRepository ??= new ActivityRepository(context);
    }

    public IActivityRecordRepository GetActivityRecordRepository()
    {
        return activityRecordRepository ??= new ActivityRecordRepository(context);
    }

    public async Task Commit()
    {
        if (isDisposed)
        {
            return;
        }

        await context?.SaveChangesAsync();
    }

    public void Dispose()
    {
        isDisposed = true;
        context?.Dispose();
        context = null;
    }
}
