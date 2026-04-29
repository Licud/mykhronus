namespace MyKhronus.DataAccess.DataUtility;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Activities.Repositories;
using MyKhronus.DataAccess.ActivityRecords.Repositories;
using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.WorkItems.Repositories;

internal class MyKhronusContextUnitOfWork(
    ILoggerFactory loggerFactory,
    IDbContextFactory<MyKhronusContext> contextFactory) 
    : IUnitOfWork
{
    private MyKhronusContext context = contextFactory.CreateDbContext();
    private ActivityRepository activityRepository;
    private ActivityRecordRepository activityRecordRepository;
    private WorkItemRepository workItemRepository;

    private bool isDisposed = false;

    public IActivityRepository GetActivityRepository()
    {
        return activityRepository ??= new ActivityRepository(context);
    }

    public IActivityRecordRepository GetActivityRecordRepository()
    {
        return activityRecordRepository ??= new ActivityRecordRepository(context);
    }

    public IWorkItemRepository GetWorkItemRepository()
    {
        var logger = loggerFactory.CreateLogger<WorkItemRepository>();

        return workItemRepository ??= new WorkItemRepository(logger, context);
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
