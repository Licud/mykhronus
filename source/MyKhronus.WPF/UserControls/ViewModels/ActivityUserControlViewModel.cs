namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

using Microsoft.Win32;

using MyKhronus.DataAccess.Services;
using MyKhronus.Models;
using MyKhronus.Models.Enums;
using MyKhronus.Models.EventArgs;
using MyKhronus.WPF.Services;
using MyKhronus.WPF.Utilities;
using MyKhronus.WPF.ViewModels;
using MyKhronus.WPF.Windows;

public class ActivityUserControlViewModel : MainViewModelControls
{
    private readonly IActivityService activityService;
    private readonly IActivityRecordsService activityRecordsService;
    private readonly IActivityRecordTimerService activityRecordTimerService;
    private readonly ILockedSessionTimerService lockedSessionTimerService;
    private readonly IAutoSaveService autoSaveService;

    private bool sessionLockStoppedTimer = false;

    public ActivityUserControlViewModel(
        IActivityRecordsService activityRecordsService,
        IActivityService activityService,
        IActivityRecordTimerService activityRecordTimerService,
        ILockedSessionTimerService lockedSessionTimerService,
        IAutoSaveService autoSaveService)
    {
        this.activityService = activityService;
        this.activityRecordsService = activityRecordsService;

        this.activityService = activityService;

        MyDay = CollectionViewSource.GetDefaultView(this.activityRecordsService.MyDay);
        MyDay.Filter = new Predicate<object>(MyDayActivityNameContains);

        RecentActivities = CollectionViewSource.GetDefaultView(this.activityRecordsService.RecentActivities);
        RecentActivities.Filter = new Predicate<object>(RecentActivityNameContains);

        MyDay.CollectionChanged += MyDay_CollectionChanged;

        this.activityRecordTimerService = activityRecordTimerService;
        this.lockedSessionTimerService = lockedSessionTimerService;
        this.autoSaveService = autoSaveService;

        this.activityRecordsService.SelectedDate = DateTime.Today;

        this.activityRecordsService.Load();

        ExecuteCollapse();

        activityService.PropertyChanged += ActivityService_PropertyChanged;
        activityRecordsService.PropertyChanged += ActivityRecordsService_PropertyChanged;
        activityRecordTimerService.PropertyChanged += ActivityRecordTimerService_PropertyChanged;

        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

        autoSaveService.StartTimer();
    }

    private bool MyDayActivityNameContains(object viewModel)
    {
        var activityRecordViewModel = viewModel as ActivityRecordViewModel;

        return activityRecordViewModel?.Name.Contains(RecentActivityFilter, StringComparison.InvariantCultureIgnoreCase) == true;
    }

    private bool RecentActivityNameContains(object viewModel)
    {
        var recentActivityViewModel = viewModel as RecentActivityViewModel;

        return recentActivityViewModel?.Name.Contains(RecentActivityFilter, StringComparison.InvariantCultureIgnoreCase) == true;
    }

    public ICollectionView MyDay { get; }

    public ICollectionView RecentActivities { get; }

    private string recentActivityFilter = "";

    public string RecentActivityFilter
    {
        get { return recentActivityFilter; }
        set
        {
            recentActivityFilter = value;
            MyDay.Refresh();
            RecentActivities.Refresh();
            OnPropertyChanged();
        }
    }

    public bool IsTimerRunning => activityRecordTimerService.IsTimerRunning;

    private string newActivityName;

    public string NewActivityName
    {
        get { return newActivityName; }
        set
        {
            newActivityName = value;
            OnPropertyChanged();
        }
    }

    public bool HasRecentActivities => activityRecordsService.HasRecentActivities;

    public DateTime SelectedDate
    {
        get { return activityRecordsService.SelectedDate; }
        set
        {
            if (activityRecordsService.SelectedDate.Date != value.Date)
            {
                activityRecordsService.SelectedDate = value;

                activityRecordsService.Load();

                ExecuteCollapse();

                OnPropertyChanged();
            }
        }
    }

    public string LastSaveTime => activityService.TimeLastSaved.ToShortTimeString();

    private TimeSpan totalDuration;

    public TimeSpan TotalDuration
    {
        get { return totalDuration; }
        set
        {
            totalDuration = value;
            OnPropertyChanged(nameof(TotalDurationDisplay));
        }
    }

    public string TotalDurationDisplay => TotalDuration.ToString();

    private ActivityRecord AddActivityRecord()
    {
        var activity = activityService.GetActivity(NewActivityName);

        if (activity == null)
        {
            activity = activityService.AddActivity(NewActivityName);
        }

        var activityRecord = activityRecordsService.MyDay
            .FirstOrDefault(day => day.ActivityId == activity.ActivityId)
            ?.ActivityRecord;

        if (activityRecord == null)
        {
            activityRecord = activityRecordsService.AddToMyDay(activity);
        }

        ExecuteSave();

        return activityRecord;
    }

    public ICommand AddNewActivity => new RelayCommand(ExecuteAddNewActivity,
        () => !string.IsNullOrWhiteSpace(NewActivityName));

    private void ExecuteAddNewActivity()
    {
        AddActivityRecord();

        NewActivityName = "";
    }

    public ICommand AddAndStartNewActivity => new RelayCommand(ExecuteAddAndStartNewActivity,
        () => !string.IsNullOrWhiteSpace(NewActivityName) && SelectedDate.Date == DateTime.Today);

    private void ExecuteAddAndStartNewActivity()
    {
        var activityRecord = AddActivityRecord();

        activityRecordTimerService.StartTimer(activityRecord);

        NewActivityName = "";
    }

    public ICommand PreviousDay => new RelayCommand(() => SelectedDate = SelectedDate.AddDays(-1));

    public ICommand NextDay => new RelayCommand(() => SelectedDate = SelectedDate.AddDays(1),
        () => SelectedDate.Date != DateTime.Today.Date);

    public ICommand Expand => new RelayCommand(ExecuteExpand);

    private void ExecuteExpand()
    {
        foreach (ActivityRecordViewModel item in MyDay)
        {
            item.IsCollapsed = false;
        }
    }

    public ICommand Collapse => new RelayCommand(ExecuteCollapse);

    private void ExecuteCollapse()
    {
        foreach (ActivityRecordViewModel item in MyDay)
        {
            item.IsCollapsed = true;
        }
    }

    public ICommand StartTimer => new RelayCommand(ExecuteStartTimer, CanExecuteStartTimer);

    private void ExecuteStartTimer() => activityRecordTimerService.StartTimer();

    private bool CanExecuteStartTimer() => !IsTimerRunning && activityRecordTimerService.HasCurrentActivityRecord;

    public ICommand PauseTimer => new RelayCommand(ExecutePauseTimer);

    private void ExecutePauseTimer() => activityRecordTimerService.StopTimer();

    public ICommand Save => new RelayCommand(ExecuteSave);

    private void ExecuteSave()
    {
        if (activityRecordTimerService.HasCurrentActivityRecord)
        {
            activityService.SaveRecord(activityRecordTimerService.CurrentActivityRecord);
        }

        activityRecordsService.Save();
    }

    private void MyDay_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var viewModel in e.NewItems.OfType<ActivityRecordViewModel>())
            {
                TotalDuration = TotalDuration.Add(viewModel.Duration);

                viewModel.ActivityRecord.DurationChanged += ActivityRecord_DurationChanged;
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var viewModel in e.OldItems.OfType<ActivityRecordViewModel>())
            {
                TotalDuration = TotalDuration.Subtract(viewModel.Duration);

                viewModel.ActivityRecord.DurationChanged -= ActivityRecord_DurationChanged;
            }
        }
    }

    private void ActivityRecord_DurationChanged(object sender, ActivityRecordDurationChangedArgs e)
    {
        if (e.DurationChangeReason == DurationChangeReason.Add)
        {
            TotalDuration = TotalDuration.Add(e.DurationChange);
        }

        if (e.DurationChangeReason == DurationChangeReason.Subtract
            || e.DurationChangeReason == DurationChangeReason.Reset)
        {
            TotalDuration = TotalDuration.Subtract(e.DurationChange);
        }
    }

    private void ActivityService_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName?.Equals(nameof(activityService.TimeLastSaved)) == true)
        {
            OnPropertyChanged(nameof(LastSaveTime));
        }
    }

    private void ActivityRecordsService_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName?.Equals(nameof(HasRecentActivities)) == true)
        {
            OnPropertyChanged(nameof(HasRecentActivities)); ;
        }
    }

    private void ActivityRecordTimerService_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName?.Equals(nameof(IsTimerRunning)) == true)
        {
            OnPropertyChanged(nameof(IsTimerRunning)); ;
        }
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.SessionLock)
        {
            if (activityRecordTimerService.IsTimerRunning)
            {
                sessionLockStoppedTimer = true;

                activityRecordTimerService.StopTimer();

                lockedSessionTimerService.StartTimer();
            }

            autoSaveService.StopTimer();
        }
        else if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            if (sessionLockStoppedTimer)
            {
                sessionLockStoppedTimer = false;

                lockedSessionTimerService.StopTimer();

                var messageBoxText = $"Would you like to add {lockedSessionTimerService.Duration} to {activityRecordTimerService.CurrentActivityRecord.Activity.Name} record on {activityRecordTimerService.CurrentActivityRecord.RecordDateOnly.ToShortDateString()}?";

                var window = new ConfirmWindow(messageBoxText, "Locked time");

                if (window.ShowDialog() == true)
                {
                    activityRecordTimerService.CurrentActivityRecord.AddDuration(lockedSessionTimerService.Duration);
                }

                lockedSessionTimerService.ResetDuration();
            }

            autoSaveService.StartTimer();
        }
    }
}
