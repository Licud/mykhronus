namespace MyKhronus.DataAccess.Services;

using System.Timers;

using MyKhronus.Commons.Utilities;
using MyKhronus.Models;

public class ActivityRecordTimerService : NotifyPropertyChanged, IActivityRecordTimerService
{
    private const int intervalInMilliseconds = 1000;

    private Timer timer = new Timer(intervalInMilliseconds);

    public ActivityRecordTimerService()
    {
        timer.Elapsed += Timer_Elapsed;
    }

    public ActivityRecord CurrentActivityRecord { get; private set; }

    private bool isTimerRunning;

    public bool IsTimerRunning
    {
        get { return isTimerRunning; }
        set
        {
            isTimerRunning = value;
            OnPropertyChanged();
        }
    }

    public bool HasCurrentActivityRecord => CurrentActivityRecord != null;

    public void StartTimer()
    {
        if (CurrentActivityRecord == null)
        {
            return;
        }

        if (CurrentActivityRecord.Duration != ActivityRecord.DurationLimit)
        {
            timer.Start();

            IsTimerRunning = true;

            CurrentActivityRecord.IsTimerRunning = true;
        }
    }

    public void StartTimer(ActivityRecord activityRecord)
    {
        if (CurrentActivityRecord?.ActivityRecordId != activityRecord.ActivityRecordId)
        {
            if (CurrentActivityRecord?.IsTimerRunning == true)
            {
                CurrentActivityRecord.IsTimerRunning = false;
            }

            CurrentActivityRecord = activityRecord;
        }

        StartTimer();
    }

    public void StopTimer()
    {
        IsTimerRunning = false;

        CurrentActivityRecord.IsTimerRunning = false;

        timer.Stop();
    }

    public void ClearCurrentActivityRecord()
    {
        StopTimer();

        CurrentActivityRecord = null;

        OnPropertyChanged(nameof(HasCurrentActivityRecord));
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e) => CurrentActivityRecord.IncrementByOneSecond();
}
