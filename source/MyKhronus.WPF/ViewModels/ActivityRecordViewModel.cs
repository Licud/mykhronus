namespace MyKhronus.WPF.ViewModels;

using System.ComponentModel;
using System.Windows.Input;

using MyKhronus.Commons.Utilities;
using MyKhronus.DataAccess.Services;
using MyKhronus.Models;
using MyKhronus.WPF.Services;
using MyKhronus.WPF.Utilities;

public class ActivityRecordViewModel : NotifyPropertyChanged
{
    private readonly IActivityRecordTimerService activityRecordTimerService;

    private readonly IActivityRecordsService activityRecordService;

    public ActivityRecordViewModel(ActivityRecord activityRecord,
        IActivityRecordsService activityRecordService,
        IActivityRecordTimerService activityRecordTimerService)
    {
        ActivityRecord = activityRecord;

        this.activityRecordTimerService = activityRecordTimerService;
        this.activityRecordService = activityRecordService;

        ActivityRecord.PropertyChanged += ActivityRecord_PropertyChanged;
    }

    public ActivityRecord ActivityRecord { get; }

    public Guid ActivityId => ActivityRecord.ActivityId;

    public Guid ActivityRecordId => ActivityRecord.ActivityRecordId;

    public string Name => ActivityRecord.Activity.Name;

    public string Description
    {
        get { return ActivityRecord.Description; }
        set
        {
            ActivityRecord.Description = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan Duration => ActivityRecord.Duration;

    public string DurationDisplay => ActivityRecord.Duration.ToString();

    public bool IsTimerRunning => ActivityRecord.IsTimerRunning;

    private bool isCollapsed = false;

    public bool IsCollapsed
    {
        get { return isCollapsed; }
        set
        {
            isCollapsed = value;
            OnPropertyChanged();
        }
    }

    private int? addMinutes = 5;

    public string AddMins
    {
        get { return addMinutes.HasValue ? addMinutes.Value.ToString() : ""; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                addMinutes = null;
            }
            else if (int.TryParse(value, out int tempMinutes))
            {
                addMinutes = tempMinutes;
            }

            OnPropertyChanged();
        }
    }

    public ICommand Collapse => new RelayCommand(() => IsCollapsed = true);

    public ICommand Expand => new RelayCommand(() => IsCollapsed = false);

    public ICommand Add => new RelayCommand(ExecuteAdd, () => addMinutes.HasValue);

    private void ExecuteAdd()
    {
        var timespan = new TimeSpan(0, addMinutes.Value, 0);

        ActivityRecord.AddDuration(timespan);
    }

    public ICommand Subtract => new RelayCommand(ExecuteSubtract, () => addMinutes.HasValue);

    private void ExecuteSubtract()
    {
        var timespan = new TimeSpan(0, addMinutes.Value, 0);

        var result = ActivityRecord.Duration.Subtract(timespan);

        if (result.Minutes >= 0)
        {
            ActivityRecord.SubtractDuration(timespan);
        }
    }

    public ICommand Reset => new RelayCommand(() => ActivityRecord.ResetDuration());

    public ICommand StartTimer => new RelayCommand(() => activityRecordTimerService.StartTimer(ActivityRecord),
        () => ActivityRecord.RecordDate.Date == DateTime.Today.Date);

    public ICommand PauseTimer => new RelayCommand(() => activityRecordTimerService.StopTimer());

    public ICommand DeleteTimer => new RelayCommand(ExecuteDeleteTimer);

    private void ExecuteDeleteTimer() => activityRecordService.DeleteFromMyDay(ActivityRecord);

    private void ActivityRecord_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(ActivityRecord.Duration)))
        {
            OnPropertyChanged(nameof(DurationDisplay));
        }

        if (e.PropertyName.Equals(nameof(ActivityRecord.IsTimerRunning)))
        {
            OnPropertyChanged(nameof(IsTimerRunning));
        }
    }

}
