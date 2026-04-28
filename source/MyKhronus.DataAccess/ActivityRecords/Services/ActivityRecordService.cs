namespace MyKhronus.DataAccess.ActivityRecords.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.ActivityRecords.Models;
using MyKhronus.DataAccess.DataUtility;

internal class ActivityRecordService(IUnitOfWork unitOfWork) : IActivityRecordService
{
    public async Task<ActivityRecord> Add(NewActivityRecord activityRecord)
    {
        var repository = unitOfWork.GetActivityRecordRepository();

        var added = await repository.Add(activityRecord);

        await unitOfWork.Commit();
    
        return added;
    }

    public async Task Delete(Guid activityRecordId)
    {
        var repository = unitOfWork.GetActivityRecordRepository();

        await repository.Delete(activityRecordId);

        await unitOfWork.Commit();
    }

    public async Task<IEnumerable<ActivityRecord>> Get(ActivityRecordFilter filter)
    {
        var repository = unitOfWork.GetActivityRecordRepository();

        return await repository.Get(filter);
    }
}
