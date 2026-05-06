namespace MyKhronus.DataAccess.DataUtility;

using MyKhronus.DataAccess.Activities.Repositories;
using MyKhronus.DataAccess.ActivityRecords.Repositories;
using MyKhronus.DataAccess.DayEntries.Repositories;
using MyKhronus.DataAccess.Projects.Repositories;
using MyKhronus.DataAccess.WorkItems.Repositories;

internal interface IUnitOfWork : IDisposable
{
    IActivityRepository GetActivityRepository();

    IActivityRecordRepository GetActivityRecordRepository();

    IWorkItemRepository GetWorkItemRepository();

    IProjectRepository GetProjectRepository();

    IDailyEntryRepository GetDailyEntryRepository();

    Task Commit();
}
