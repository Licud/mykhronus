namespace MyKhronus.DataAccess.Activities.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MyKhronus.DataAccess.Activities.Models;
using MyKhronus.DataAccess.Context;

internal class ActivityRepository(MyKhronusContext context) : IActivityRepository
{
    public Task<Activity> Add(NewActivity activity)
    {
        var added = context.Activities.Add(new Entities.Activity()
        {
            ActivityId = Guid.NewGuid(),
            Name = activity.Name,
        });

        var model = new Activity()
        {
            ActivityId = added.Entity.ActivityId,
            Name = activity.Name,
        };

        return Task.FromResult(model);
    }

    public async Task Delete(Guid activityId)
    {
        var activity = await context.Activities.FindAsync(activityId);

        if (activity != null)
        {
            context.Activities.Remove(activity);
        }
    }

    public async Task<IEnumerable<Activity>> Get(ActivityFilter filter)
    {
        var query = context.Activities.AsQueryable();

        if (filter.Id.HasValue)
        {
            query = query.Where(a => a.ActivityId == filter.Id.Value);
        }

        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(a => a.Name == filter.Name);
        }

        if (filter.WithRecordsOnDate.HasValue)
        {
            var recordDate = filter.WithRecordsOnDate.Value;

            query = query
                .Where(a => a.Records.Any(r => r.RecordDate >= recordDate && r.RecordDate < recordDate.AddDays(1)));
        }

        var models = await query
            .Select(a => new Activity()
            {
                ActivityId = a.ActivityId,
                Name = a.Name,
            })
            .ToListAsync();

        return models;
    }
}
