namespace MyKhronus.WPF.Services;

using System.Collections.Specialized;

using MyKhronus.Commons.Utilities;
using MyKhronus.DataAccess.Services;
using MyKhronus.Models;
using MyKhronus.WPF.UIModels;
using MyKhronus.WPF.Windows;

public class ActivityRecordsService : NotifyPropertyChanged, IActivityRecordsService
{
    private readonly IActivityService activityService;

    private readonly IActivityRecordTimerService activityRecordTimerService;

    public ActivityRecordsService(IActivityService activityService,
        IActivityRecordTimerService activityRecordTimerService)
    {
        this.activityService = activityService;
        this.activityRecordTimerService = activityRecordTimerService;

        MyDay = new ActivityRecordCollection(this, this.activityRecordTimerService);
        RecentActivities = new RecentActivityCollection(this, activityRecordTimerService);

        RecentActivities.CollectionChanged += RecentActivities_CollectionChanged;
    }

    public DateTime SelectedDate { get; set; } = DateTime.Today;

    public ActivityRecordCollection MyDay { get; }

    public RecentActivityCollection RecentActivities { get; }

    public bool HasRecentActivities => RecentActivities.Any();

    public void Load()
    {
        RecentActivities.Clear();
        MyDay.Clear();

        var recentActivities = activityService
            .GetActivitiesWithoutRecordsOn(SelectedDate.Date)
            .ToList();

        RecentActivities.LoadItems(recentActivities);

        var records = activityService.GetActivityRecords(SelectedDate.Date);

        var recordsToLoad = records.ToList();

        if (activityRecordTimerService.HasCurrentActivityRecord 
            && activityRecordTimerService.CurrentActivityRecord.RecordDate == SelectedDate.Date)
        {
            recordsToLoad = records
                .Where(tm => tm.ActivityRecordId != activityRecordTimerService.CurrentActivityRecord.ActivityRecordId)
                .ToList();

            recordsToLoad.Insert(0, activityRecordTimerService.CurrentActivityRecord);
        }

        MyDay.LoadItems(recordsToLoad);

    }

    public ActivityRecord AddToMyDay(Activity activity)
    {
        var recentActivity = RecentActivities
            .FirstOrDefault(r => r.ActivityId == activity.ActivityId);

        RecentActivities.Remove(recentActivity);

        var record = activityService.AddRecord(activity.ActivityId, SelectedDate);

        MyDay.Add(record);

        return record;
    }

    public void AddToMyDay(ActivityRecord record)
    {
        MyDay.Add(record);
    }

    public void DeleteActivity(Activity activity)
    {
        var messageBoxText = $@"This would delete ALL records of this Activity. 
Would you like to continue?";

        var window = new ConfirmWindow(messageBoxText, "Delete All Records");

        if (window.ShowDialog() == true)
        {
            if (activityRecordTimerService.CurrentActivityRecord?.ActivityId == activity.ActivityId)
            {
                activityRecordTimerService.ClearCurrentActivityRecord();
            }

            RecentActivities.Remove(activity);

            activityService.Delete(activity.ActivityId);
        }
    }

    public void DeleteFromMyDay(ActivityRecord activityRecord)
    {
        if (activityRecord.IsTimerRunning)
        {
            activityRecordTimerService.ClearCurrentActivityRecord();
        }

        RecentActivities.Add(activityRecord.Activity);

        MyDay.Remove(activityRecord);

        activityService.DeleteRecord(activityRecord.ActivityRecordId);
    }

    public void Save()
    {
        foreach (var item in MyDay)
        {
            activityService.SaveRecord(item.ActivityRecord);
        }
    }

    private void RecentActivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => OnPropertyChanged(nameof(HasRecentActivities));

}
