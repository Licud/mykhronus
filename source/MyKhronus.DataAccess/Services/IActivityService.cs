namespace MyKhronus.DataAccess.Services;

using System.Collections.Generic;
using System.ComponentModel;

using MyKhronus.Models;

public interface IActivityService : INotifyPropertyChanged
{
    DateTime TimeLastSaved { get; set; }

    Activity GetActivity(Guid id);

    Activity GetActivity(string name);

    Activity AddActivity(string name);

    ActivityRecord GetActivityRecord(Guid activityId, DateTime recordDate);

    ActivityRecord AddRecord(Guid activityId, DateTime recordDate);

    IEnumerable<Activity> GetActivitiesWithoutRecordsOn(DateTime dateTime);

    IEnumerable<ActivityRecord> GetActivityRecords(DateTime selectedDate);

    void Delete(Guid activityId);

    void DeleteRecord(Guid activityRecordId);

    void SaveRecord(ActivityRecord activityRecord);

    IEnumerable<ActivityRecord> GetRecordsBetween(DateTime from, DateTime to);
}
