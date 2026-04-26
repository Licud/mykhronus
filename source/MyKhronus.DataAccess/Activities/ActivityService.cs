namespace MyKhronus.DataAccess.Activities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyKhronus.DataAccess.Activities.Models;

internal class ActivityService : IActivityService
{
    public Task<Activity> Add(NewActivity activity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid activityId)
    {
        throw new NotImplementedException();
    }

    public Task<Activity> Get(Guid activityId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Activity>> Get(ActivityFilter filter)
    {
        throw new NotImplementedException();
    }
}
