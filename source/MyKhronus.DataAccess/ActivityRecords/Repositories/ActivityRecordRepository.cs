namespace MyKhronus.DataAccess.ActivityRecords.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MyKhronus.DataAccess.Activities.Models;
using MyKhronus.DataAccess.ActivityRecords.Models;
using MyKhronus.DataAccess.Context;

internal class ActivityRecordRepository(MyKhronusContext context) : IActivityRecordRepository
{
    public Task<ActivityRecord> Add(NewActivityRecord activityRecord)
    {
        var added = context.ActivityRecords.Add(new Entities.ActivityRecord()
        {
            ActivityRecordId = Guid.NewGuid(),
            RecordDate = activityRecord.RecordDate,
            Description = activityRecord.Description,
            Duration = activityRecord.Duration,
            ActivityId = activityRecord.ActivityId,
        });

        var model = new ActivityRecord()
        {
            ActivityRecordId = added.Entity.ActivityRecordId,
            RecordDate = activityRecord.RecordDate,
            Description = activityRecord.Description,
            Duration = activityRecord.Duration,
        };

        return Task.FromResult(model);
    }

    public async Task Delete(Guid activityRecordId)
    {
        var activityRecord = await context.ActivityRecords.FindAsync(activityRecordId);

        if (activityRecord != null)
        {
            context.ActivityRecords.Remove(activityRecord);
        }
    }

    public async Task<IEnumerable<ActivityRecord>> Get(ActivityRecordFilter filter)
    {
        var query = context.ActivityRecords.AsQueryable();

        if (filter.ActivityId.HasValue)
        {
            query = query.Where(r => r.ActivityId == filter.ActivityId.Value);
        }

        if (filter.RecordDate.HasValue)
        {
            var recordDate = filter.RecordDate.Value.Date;

            query = query
                .Where(r => r.RecordDate >= recordDate && r.RecordDate < recordDate.AddDays(1));
        }

        if (filter.RecordDateRange.HasValue)
        {
            query = query
                .Where(r => r.RecordDate >= filter.RecordDateRange.Value.From
                         && r.RecordDate < filter.RecordDateRange.Value.To);
        }

        var models = await query
            .Select(r => new ActivityRecord()
            {
                ActivityRecordId = r.ActivityRecordId,
                RecordDate = r.RecordDate,
                Description = r.Description,
                Duration = r.Duration,
                Activity = new Activity()
                {
                    ActivityId = r.Activity.ActivityId,
                    Name = r.Activity.Name,
                },
            })
            .ToListAsync();

        return models;
    }
}
