namespace MyKhronus.DataAccess.ActivityRecords.Services;

using MyKhronus.DataAccess.ActivityRecords.Models;

public interface IActivityRecordService
{
    Task<ActivityRecord> Add(NewActivityRecord activityRecord);

    Task<IEnumerable<ActivityRecord>> Get(ActivityRecordFilter filter);

    Task Delete(Guid activityRecordId);
}
