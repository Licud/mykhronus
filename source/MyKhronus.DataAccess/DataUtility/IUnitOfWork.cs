namespace MyKhronus.DataAccess.DataUtility;

using MyKhronus.DataAccess.DayEntries.Repositories;
using MyKhronus.DataAccess.Projects.Repositories;
using MyKhronus.DataAccess.ScheduledWorkItems.Repositories;
using MyKhronus.DataAccess.WorkItems.Repositories;

internal interface IUnitOfWork : IDisposable
{

    IWorkItemRepository WorkItems { get; }

    IProjectRepository Projects { get; }

    IDailyEntryRepository DailyEntries { get; }

    IScheduledWorkItemRepository GetScheduledWorkItemRepository();

    Task Commit();
}
