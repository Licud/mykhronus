namespace MyKhronus.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MyKhronus.Commons.Utilities;
using MyKhronus.Models.Enums;
using MyKhronus.Models.EventArgs;

public class ActivityRecord : NotifyPropertyChanged
{
    public static TimeSpan DurationLimit = new TimeSpan(23, 59, 59);

    private int MaxHoursInADay = 24;

    public event EventHandler<ActivityRecordDurationChangedArgs> DurationChanged;

    internal ActivityRecord()
    {

    }

    internal ActivityRecord(Activity activity, DateTime recordDate)
    {
        ActivityRecordId = Guid.NewGuid();
        RecordDate = recordDate;
        ActivityId = activity.ActivityId;

        Activity = activity;
    }

    public Guid ActivityRecordId { get; private set; }

    [Required]
    public DateTime RecordDate { get; private set; }

    public string Description { get; set; }

    private TimeSpan duration = new TimeSpan();

    [Required]
    public TimeSpan Duration
    {
        get { return duration; }
        private set
        {
            duration = value;
            OnPropertyChanged();
        }
    }

    private bool isTimerRunning;

    [NotMapped]
    public bool IsTimerRunning
    {
        get { return isTimerRunning; }
        set
        {
            isTimerRunning = value;
            OnPropertyChanged();
        }
    }

    [Required]
    public Guid ActivityId { get; private set; }

    public DateTime RecordDateOnly => RecordDate.Date;

    public Activity Activity { get; private set; }

    public void IncrementByOneSecond()
    {
        var oneSecondTimespan = new TimeSpan(0, 0, 1);

        AddDuration(oneSecondTimespan);
    }

    public void AddDuration(TimeSpan timespan)
    {
        var resultingDuration = Duration.Add(timespan);

        if (resultingDuration.Days < 1 && resultingDuration.Hours < MaxHoursInADay)
        {
            Duration = resultingDuration;
        }
        else
        {
            Duration = DurationLimit;
        }

        var args = new ActivityRecordDurationChangedArgs()
        {
            DurationChange = timespan,
            DurationChangeReason = DurationChangeReason.Add
        };

        OnDurationChanged(args);
    }

    public void SubtractDuration(TimeSpan timespan)
    {
        Duration = Duration.Subtract(timespan);

        var args = new ActivityRecordDurationChangedArgs()
        {
            DurationChange = timespan,
            DurationChangeReason = DurationChangeReason.Subtract
        };

        OnDurationChanged(args);
    }

    public void ResetDuration()
    {
        var oldDuration = Duration;

        Duration = new TimeSpan(0, 0, 0);

        var args = new ActivityRecordDurationChangedArgs()
        {
            DurationChange = oldDuration,
            DurationChangeReason = DurationChangeReason.Reset
        };

        OnDurationChanged(args);
    }

    protected virtual void OnDurationChanged(ActivityRecordDurationChangedArgs args)
        => DurationChanged?.Invoke(this, args);

}
