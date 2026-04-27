namespace MyKhronus.DataAccess.DataUtility;

using MyKhronus.DataAccess.Activities.Repositories;
using MyKhronus.DataAccess.ActivityRecords.Repositories;

internal interface IUnitOfWork : IDisposable
{
    IActivityRepository GetActivityRepository();

    IActivityRecordRepository GetActivityRecordRepository();

    Task Commit();
}
