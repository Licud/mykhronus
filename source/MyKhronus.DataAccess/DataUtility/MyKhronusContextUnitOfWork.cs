namespace MyKhronus.DataAccess.DataUtility;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.DayEntries.Repositories;
using MyKhronus.DataAccess.Projects.Repositories;
using MyKhronus.DataAccess.ScheduledWorkItems.Repositories;
using MyKhronus.DataAccess.WorkItems.Repositories;

internal class MyKhronusContextUnitOfWork(
    ILoggerFactory loggerFactory,
    IDbContextFactory<MyKhronusContext> contextFactory)
    : IUnitOfWork
{
    private MyKhronusContext context = contextFactory.CreateDbContext();
    private WorkItemRepository workItemRepository;
    private ProjectRepository projectRepository;
    private DailyEntryRepository dailyEntryRepository;
    private ScheduledWorkItemRepository scheduledWorkItemRepository;

    private bool isDisposed = false;

    public IWorkItemRepository WorkItems => GetWorkItemRepository();

    public IProjectRepository Projects => GetProjectRepository();

    public IDailyEntryRepository DailyEntries => GetDailyEntryRepository();

    private IWorkItemRepository GetWorkItemRepository()
    {
        var logger = loggerFactory.CreateLogger<WorkItemRepository>();

        return workItemRepository ??= new WorkItemRepository(logger, context);
    }

    private IProjectRepository GetProjectRepository()
    {
        var logger = loggerFactory.CreateLogger<ProjectRepository>();

        return projectRepository ??= new ProjectRepository(logger, context);
    }

    private IDailyEntryRepository GetDailyEntryRepository()
    {
        var logger = loggerFactory.CreateLogger<DailyEntryRepository>();

        return dailyEntryRepository ??= new DailyEntryRepository(logger, context);
    }

    public IScheduledWorkItemRepository GetScheduledWorkItemRepository()
    {
        var logger = loggerFactory.CreateLogger<ScheduledWorkItemRepository>();

        return scheduledWorkItemRepository ??= new ScheduledWorkItemRepository(logger, context);
    }

    public async Task Commit()
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        isDisposed = true;
        context?.Dispose();
        context = null;
    }
}
