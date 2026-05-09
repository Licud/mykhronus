namespace MyKhronus.DataAccess.DataUtility;

using MyKhronus.DataAccess.DayEntries.Repositories;
using MyKhronus.DataAccess.Projects.Repositories;
using MyKhronus.DataAccess.WorkItems.Repositories;

internal interface IUnitOfWork : IDisposable
{
    IWorkItemRepository GetWorkItemRepository();

    IProjectRepository GetProjectRepository();

    IDailyEntryRepository GetDailyEntryRepository();

    Task Commit();
}
