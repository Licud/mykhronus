namespace MyKhronus.DataAccess.Services;

using Microsoft.EntityFrameworkCore;

using MyKhronus.Commons.Utilities;
using MyKhronus.DataAccess.Context;
using MyKhronus.Models;

public class ActivityServices : NotifyPropertyChanged, IActivityService
{
    private readonly IDbContextFactory<MyKhronusContext_old> contextFactory;

    public ActivityServices(IDbContextFactory<MyKhronusContext_old> contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    private DateTime timeLastSaved;

    public DateTime TimeLastSaved
    {
        get { return timeLastSaved; }
        set
        {
            timeLastSaved = value;
            OnPropertyChanged();
        }
    }

    public Activity GetActivity(Guid id)
    {
        using var context = contextFactory.CreateDbContext();

        return context.Activities.FirstOrDefault(a => a.ActivityId == id);
    }

    public Activity GetActivity(string name)
    {
        using var context = contextFactory.CreateDbContext();

        return context.Activities.FirstOrDefault(a => a.Name == name);
    }

    public ActivityRecord AddRecord(Guid activityId, DateTime recordDate)
    {
        using var context = contextFactory.CreateDbContext();

        ActivityRecord newRecord = null;

        var activityRecord = GetActivityRecord(activityId, recordDate);

        if (activityRecord == null)
        {
            var activity = context.Activities.FirstOrDefault(a => a.ActivityId == activityId);

            newRecord = activity.AddRecord(recordDate);

            context.ActivityRecords.Add(newRecord);

            context.SaveChanges();
        }

        return newRecord;
    }

    public IEnumerable<Activity> GetActivitiesWithoutRecordsOn(DateTime recordDate)
    {
        using var context = contextFactory.CreateDbContext();

        return context.Activities
            .Where(a => !a.Records.Any(r => r.RecordDate.Date == recordDate.Date))
            .ToList();
    }

    public ActivityRecord GetActivityRecord(Guid activityRecordId, DateTime recordDate)
    {
        using var context = contextFactory.CreateDbContext();

        return context.ActivityRecords
            .Where(ar => ar.ActivityRecordId == activityRecordId && ar.RecordDate.Date == recordDate.Date)
            .Include(ar => ar.Activity)
            .FirstOrDefault();
    }

    public Activity AddActivity(string name)
    {
        using var context = contextFactory.CreateDbContext();

        var activity = new Activity(name);

        context.Activities.Add(activity);

        context.SaveChanges();

        return activity;
    }

    public IEnumerable<ActivityRecord> GetActivityRecords(DateTime selectedDate)
    {
        using var context = contextFactory.CreateDbContext();

        return context.ActivityRecords
            .Where(ar => ar.RecordDate.Date == selectedDate.Date)
            .Include(r => r.Activity)
            .ToList();
    }

    public void Delete(Guid activityId)
    {
        using var context = contextFactory.CreateDbContext();

        var activity = GetActivity(activityId);

        context.Remove(activity);

        context.SaveChanges();
    }

    public void DeleteRecord(Guid activityRecordId)
    {
        using var context = contextFactory.CreateDbContext();

        var activityRecord = context.ActivityRecords.Find(activityRecordId);

        context.Remove(activityRecord);

        context.SaveChanges();
    }

    public void SaveRecord(ActivityRecord activityRecord)
    {
        using var context = contextFactory.CreateDbContext();

        var record = GetActivityRecord(activityRecord.ActivityRecordId, activityRecord.RecordDate);

        record.Description = activityRecord.Description;
        record.ResetDuration();
        record.AddDuration(activityRecord.Duration);

        context.Update(activityRecord);

        context.SaveChanges();

        TimeLastSaved = DateTime.Now;
    }

    public IEnumerable<ActivityRecord> GetRecordsBetween(DateTime from, DateTime to)
    {
        using var context = contextFactory.CreateDbContext();

        return context.ActivityRecords
            .Where(ar => ar.RecordDate.Date >= from.Date && ar.RecordDate.Date <= to.Date)
            .Include(ar => ar.Activity)
            .AsNoTracking()
            .ToList();
    }

}
