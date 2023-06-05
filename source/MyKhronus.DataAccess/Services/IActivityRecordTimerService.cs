namespace MyKhronus.DataAccess.Services;

using System.ComponentModel;

using MyKhronus.Models;

public interface IActivityRecordTimerService : INotifyPropertyChanged
{
    bool IsTimerRunning { get; }

    bool HasCurrentActivityRecord { get; }

    ActivityRecord CurrentActivityRecord { get; }

    void StartTimer();

    void StartTimer(ActivityRecord activityRecord);

    void StopTimer();

    void ClearCurrentActivityRecord();
}
