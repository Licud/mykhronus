namespace MyKhronus.DataAccess.Activities.Services;

using MyKhronus.DataAccess.Activities.Models;

public interface IActivityService
{
    Task<Activity> Add(NewActivity activity);

    Task<Activity> Get(Guid activityId);

    Task<IEnumerable<Activity>> Get(ActivityFilter filter);

    Task Delete(Guid activityId);
}