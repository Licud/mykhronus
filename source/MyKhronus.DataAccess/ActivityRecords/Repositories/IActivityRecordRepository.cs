namespace MyKhronus.DataAccess.ActivityRecords.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.ActivityRecords.Models;

internal interface IActivityRecordRepository
{
    Task<ActivityRecord> Add(NewActivityRecord activityRecord);

    Task<IEnumerable<ActivityRecord>> Get(ActivityRecordFilter filter);

    Task Delete(Guid activityRecordId);
}
