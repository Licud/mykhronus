namespace MyKhronus.DataAccess.Activities.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyKhronus.DataAccess.Activities.Models;

internal interface IActivityRepository
{
    Task<Activity> Add(NewActivity activity);

    Task<IEnumerable<Activity>> Get(ActivityFilter filter);

    Task Delete(Guid activityId);
}
