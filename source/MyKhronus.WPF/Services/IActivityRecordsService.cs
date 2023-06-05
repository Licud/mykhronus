namespace MyKhronus.WPF.Services;

using System.ComponentModel;

using MyKhronus.Models;
using MyKhronus.WPF.UIModels;

public interface IActivityRecordsService : INotifyPropertyChanged
{
    bool HasRecentActivities { get; }

    ActivityRecordCollection MyDay { get; }

    RecentActivityCollection RecentActivities { get; }

    DateTime SelectedDate { get; set; }

    ActivityRecord AddToMyDay(Activity activity);

    void AddToMyDay(ActivityRecord record);

    void DeleteActivity(Activity activity);

    void DeleteFromMyDay(ActivityRecord activityRecord);

    void Load();

    void Save();
}
