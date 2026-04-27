namespace MyKhronus.DataAccess.Activities.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyKhronus.DataAccess.Activities.Models;
using MyKhronus.DataAccess.DataUtility;

internal class ActivityService(IUnitOfWork unitOfWork) : IActivityService
{
    public async Task<Activity> Add(NewActivity activity)
    {
        var repository = unitOfWork.GetActivityRepository();

        var added = await repository.Add(activity);

        await unitOfWork.Commit();

        return added;
    }

    public async Task Delete(Guid activityId)
    {
        var repository = unitOfWork.GetActivityRepository();

        await repository.Delete(activityId);

        await unitOfWork.Commit();
    }

    public async Task<Activity> Get(Guid activityId)
    {
        var repository = unitOfWork.GetActivityRepository();

        var results = await repository.Get(new ActivityFilter() { Id = activityId });

        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<Activity>> Get(ActivityFilter filter)
    {
        var repository = unitOfWork.GetActivityRepository();

        return await repository.Get(filter);
    }
}
