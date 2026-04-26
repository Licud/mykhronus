using MyKhronus.DataAccess.ActivityRecords.Models;

namespace MyKhronus.DataAccess.ActivityRecords
{
    public interface IActivityRecordService
    {
        Task<ActivityRecord> Add(NewActivityRecord activityRecord);

        Task<IEnumerable<ActivityRecord>> Get(ActivityRecordFilter filter);

        Task Delete(Guid activityRecordId);
    }
}
