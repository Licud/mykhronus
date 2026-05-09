namespace MyKhronus.DataAccess.DataUtility;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.DayEntries.Repositories;
using MyKhronus.DataAccess.Projects.Repositories;
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

    private bool isDisposed = false;

    public IWorkItemRepository GetWorkItemRepository()
    {
        var logger = loggerFactory.CreateLogger<WorkItemRepository>();

        return workItemRepository ??= new WorkItemRepository(logger, context);
    }

    public IProjectRepository GetProjectRepository()
    {
        var logger = loggerFactory.CreateLogger<ProjectRepository>();

        return projectRepository ??= new ProjectRepository(logger, context);
    }

    public IDailyEntryRepository GetDailyEntryRepository()
    {
        var logger = loggerFactory.CreateLogger<DailyEntryRepository>();

        return dailyEntryRepository ??= new DailyEntryRepository(logger, context);
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
