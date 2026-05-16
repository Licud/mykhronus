namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Win32;

using MyKhronus.DataAccess.DayEntries.Services;
using MyKhronus.DataAccess.Projects.Services;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Builders;
using MyKhronus.WPF.Enums;
using MyKhronus.WPF.UserControls.EventArguments;
using MyKhronus.WPF.Utilities;

public class DayUserControlViewModel : MainViewModelControls, IDisposable
{
    private readonly IWorkItemService workItemService;
    private readonly IDailyEntryService dailyEntryService;
    private readonly IProjectService projectService;
    private readonly ProjectPickerViewModelFactory projectPickerViewModelFactory;
    private readonly DayEntryViewModelFactory dayEntryViewModelFactory;
    private readonly RecentWorkItemViewModelFactory recentWorkItemViewModelFactory;

    private readonly ObservableCollection<DayEntryViewModel> myDayEntries = [];

    private DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private DispatcherTimer autoSaveTimer = new() { Interval = TimeSpan.FromMinutes(3) };

    private DayEntryViewModel currentlyRunningTimerEntry;

    private DateTime? sessionLockTime;

    private DateTime? lastSaveTime;

    private bool isAddingToMyDay;

    private bool isRecentsViewActive = true;

    public DayUserControlViewModel(
        IWorkItemService workItemService,
        IDailyEntryService dailyEntryService,
        IProjectService projectService,
        ProjectPickerViewModelFactory projectPickerViewModelFactory,
        DayEntryViewModelFactory dayEntryViewModelFactory,
        RecentWorkItemViewModelFactory recentWorkItemViewModelFactory,
        RecentsViewModel recents,
        ScheduledViewModel scheduled)
    {
        this.workItemService = workItemService;
        this.dailyEntryService = dailyEntryService;
        this.projectService = projectService;
        this.projectPickerViewModelFactory = projectPickerViewModelFactory;
        this.dayEntryViewModelFactory = dayEntryViewModelFactory;
        this.recentWorkItemViewModelFactory = recentWorkItemViewModelFactory;

        Recents = recents;
        Scheduled = scheduled;

        Recents.IsAlreadyInMyDay = id => myDayEntries.Any(e => e.WorkItemId == id);
        Recents.AddToMyDayRequested += Recents_AddToMyDayRequested;
        Recents.StartAndAddToMyDayRequested += Recents_StartAndAddToMyDayRequested;

        MyDay = CollectionViewSource.GetDefaultView(myDayEntries);
        MyDay.Filter = new Predicate<object>(DayEntryNameContains);

        selectedDate = DateTime.Today;

        timer.Tick += Timer_Tick;

        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

        autoSaveTimer.Tick += async (_, _) => await ExecuteSave();
        autoSaveTimer.Start();
    }

    // Properties

    public RecentsViewModel Recents { get; }

    public ScheduledViewModel Scheduled { get; }

    public ICollectionView MyDay { get; }

    public bool IsRecentsViewActive
    {
        get => isRecentsViewActive;
        private set
        {
            isRecentsViewActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsScheduledViewActive));
        }
    }

    public bool IsScheduledViewActive => !isRecentsViewActive;

    public ICommand ShowRecents => new RelayCommand(() => IsRecentsViewActive = true);

    public ICommand ShowScheduled => new RelayCommand(() => IsRecentsViewActive = false);

    private string workItemFilter = "";

    public string WorkItemFilter
    {
        get { return workItemFilter; }
        set
        {
            workItemFilter = value;
            Recents.SetFilter(value);
            MyDay.Refresh();
            OnPropertyChanged();
        }
    }

    private string newEntryName;

    public string NewEntryName
    {
        get => newEntryName;
        set
        {
            newEntryName = value;
            OnPropertyChanged();
        }
    }

    private DateTime selectedDate;

    public DateTime SelectedDate
    {
        get => selectedDate;
        set
        {
            if (selectedDate.Date != value.Date)
            {
                selectedDate = value;
                AsyncWrapper(ReloadCollections);
                OnPropertyChanged();
            }
        }
    }

    private TimeSpan totalDuration;

    public TimeSpan TotalDuration
    {
        get => totalDuration;
        set
        {
            totalDuration = value;
            OnPropertyChanged(nameof(TotalDurationDisplay));
        }
    }

    private ProjectPickerViewModel projectPicker;

    public ProjectPickerViewModel ProjectPicker
    {
        get { return projectPicker; }
        set 
        { 
            projectPicker = value; 
            OnPropertyChanged();
        }
    }


    public string TotalDurationDisplay => TotalDuration.ToString();

    public string LastSaveTime => lastSaveTime?.ToShortTimeString() ?? "";

    public bool IsTimerRunning => timer.IsEnabled;

    public ICommand StartTimer => new RelayCommand(ExecuteGlobalStartTimer, CanExecuteGlobalStartTimer);

    public ICommand PauseTimer => new RelayCommand(ExecuteGlobalPauseTimer, () => IsTimerRunning);

    public ICommand Save => new RelayCommand(async () => await ExecuteSave());

    public ICommand Loaded => new RelayCommand(async () => await InitializeLoad());

    public ICommand AddNewEntry => new RelayCommand(async () => await ExecuteAddNewEntry(), CanAddNewEntry);

    public ICommand AddAndStartNewEntry => new RelayCommand(async () => await ExecuteAddAndStartNewEntry(), CanAddAndStartNewEntry);

    public ICommand PreviousDay => new RelayCommand(() => SelectedDate = SelectedDate.AddDays(-1));

    public ICommand NextDay => new RelayCommand(() => SelectedDate = SelectedDate.AddDays(1),
        () => SelectedDate.Date != DateTime.Today.Date);

    private void ExecuteGlobalStartTimer() => StartTimerForEntry(currentlyRunningTimerEntry);

    private bool CanExecuteGlobalStartTimer() => !IsTimerRunning && currentlyRunningTimerEntry != null;

    private void ExecuteGlobalPauseTimer()
    {
        if (currentlyRunningTimerEntry == null)
        {
            return;
        }

        currentlyRunningTimerEntry.IsTimerRunning = false;
        timer.Stop();

        OnPropertyChanged(nameof(IsTimerRunning));
    }

    private async Task ExecuteSave()
    {
        foreach (var entry in myDayEntries.ToList())
        {
            await dailyEntryService.Update(entry.DayEntry);
        }

        if (currentlyRunningTimerEntry != null && !myDayEntries.Contains(currentlyRunningTimerEntry))
        {
            await dailyEntryService.Update(currentlyRunningTimerEntry.DayEntry);
        }

        lastSaveTime = DateTime.Now;

        OnPropertyChanged(nameof(LastSaveTime));
    }

    // Recent Work Item Methods

    private async Task ExecuteAddNewEntry()
    {
        try
        {
            await AddWorkItem();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    }

    private async Task ExecuteAddAndStartNewEntry()
    {
        try
        {
            var entry = await AddWorkItem();

            await AddWorkItemToMyDay(entry, startTimer: true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    }

    private bool IsToday() => selectedDate.Date == DateTime.Today;

    private bool CanAddNewEntry() => !string.IsNullOrWhiteSpace(NewEntryName);

    private bool CanAddAndStartNewEntry() => CanAddNewEntry() && IsToday();

    private async Task<WorkItem> AddWorkItem()
    {
        var newName = NewEntryName.Trim();

        var filter = new WorkItemGetFilter()
        {
            Description = newName,
        };

        var savedWorkItems = await workItemService.Get(filter);

        var savedWorkItem = savedWorkItems
            .FirstOrDefault(w => w.Description.Equals(newName, StringComparison.InvariantCultureIgnoreCase));

        if (savedWorkItem == null)
        {
            var newWorkItem = new NewWorkItem(newName);

            savedWorkItem = await workItemService.Add(newWorkItem);

            if (projectPicker.HasSelectedProject)
            {
                await workItemService.LinkWorkItemToProject(savedWorkItem.Id, projectPicker.SelectedProject.Id);

                savedWorkItem = savedWorkItem with { Project = projectPicker.SelectedProject };
            }
        }

        var inRecents = Recents.Contains(savedWorkItem.Id);
        var inMyDayEntries = myDayEntries.Any(e => e.WorkItemId == savedWorkItem.Id);

        if (!inRecents && !inMyDayEntries)
        {
            var recent = await recentWorkItemViewModelFactory.CreateRecentWorkItemViewModel(savedWorkItem, IsToday());

            Recents.Add(recent);
        }
        else
        {
            WorkItemFilter = newName;
        }

        NewEntryName = string.Empty;

        return savedWorkItem;
    }

    private bool DayEntryNameContains(object viewModel)
    {
        var dayEntryViewModel = viewModel as DayEntryViewModel;

        return dayEntryViewModel?.Name.Contains(WorkItemFilter, StringComparison.InvariantCultureIgnoreCase) == true;
    }

    // Day Entry Methods

    private async Task AddWorkItemToMyDay(WorkItem workItem, bool startTimer = false)
    {
        if (isAddingToMyDay)
        {
            return;
        }

        isAddingToMyDay = true;

        try
        {
            var currentDayEntry = myDayEntries.FirstOrDefault(e => e.WorkItemId == workItem.Id);

            if (currentDayEntry == null)
            {
                var dayEntry = await dailyEntryService.Add(selectedDate, workItem.Id);

                var projectPicker = await projectPickerViewModelFactory.CreateProjectPickerViewModel();

                currentDayEntry = await dayEntryViewModelFactory.CreateDayEntryViewModel(dayEntry, workItem);

                AddDayEntry(currentDayEntry);

                var existing = Recents.FindById(workItem.Id);

                if (existing != null)
                {
                    Recents.Remove(existing);
                }
            }

            if (startTimer)
            {
                StartTimerForEntry(currentDayEntry);
            }
        }
        finally
        {
            isAddingToMyDay = false;
        }
    }

    private void AddDayEntry(DayEntryViewModel viewModel, int? index = null)
    {
        viewModel.Deleted += DayEntry_Deleted;
        viewModel.DurationChanged += DayEntry_DurationChanged;
        viewModel.TimerStateChanged += DayEntry_TimerStateChanged;

        if (index.HasValue)
        {
            myDayEntries.Insert(index.Value, viewModel);
        }
        else
        {
            myDayEntries.Add(viewModel);
        }
        TotalDuration = TotalDuration.Add(viewModel.Duration);
    }

    private void RemoveDayEntry(DayEntryViewModel viewModel)
    {
        viewModel.Deleted -= DayEntry_Deleted;
        viewModel.DurationChanged -= DayEntry_DurationChanged;
        viewModel.TimerStateChanged -= DayEntry_TimerStateChanged;

        myDayEntries.Remove(viewModel);

        TotalDuration = TotalDuration.Subtract(viewModel.Duration);
    }

    private async Task InitializeLoad()
    {
        await ReloadCollections();
        ProjectPicker = await projectPickerViewModelFactory.CreateProjectPickerViewModel();
    }

    private async Task ReloadCollections()
    {
        Recents.Clear();

        foreach (var entry in myDayEntries.ToList())
        {
            RemoveDayEntry(entry);
        }

        TotalDuration = TimeSpan.Zero;

        Recents.IsToday = IsToday();

        var workItems = Task.Run(() => workItemService.Get(new())).GetAwaiter().GetResult();
        var dayEntries = Task.Run(() => dailyEntryService.GetEntries(selectedDate)).GetAwaiter().GetResult();

        var dayEntryWorkItemIds = dayEntries.Select(e => e.WorkItemId).ToHashSet();

        var selectableWorkItems = workItems.Where(w => !dayEntryWorkItemIds.Contains(w.Id)).ToList();

        foreach (var item in selectableWorkItems)
        {
            var recent = await recentWorkItemViewModelFactory.CreateRecentWorkItemViewModel(item, IsToday());

            Recents.Add(recent);
        }

        var loadedWorkItems = workItems.ToDictionary(w => w.Id, w => w);

        var isToday = selectedDate.Date == DateTime.Today;
        var runningWorkItemId = isToday ? currentlyRunningTimerEntry?.WorkItemId : null;

        var dayEntriesExcludingCurrentlyRunning = dayEntries.Where(e => e.WorkItemId != runningWorkItemId);

        foreach (var dayEntry in dayEntriesExcludingCurrentlyRunning)
        {
            var workItem = Task.Run(() => ResolveWorkItem(loadedWorkItems, dayEntry.WorkItemId)).GetAwaiter().GetResult();
            var viewModel = await dayEntryViewModelFactory.CreateDayEntryViewModel(dayEntry, workItem);

            AddDayEntry(viewModel);
        }

        var runningEntryExistsInDayEntries = dayEntries.Any(e => e.WorkItemId == currentlyRunningTimerEntry?.WorkItemId);

        var runningEntryIsOnThisDate = isToday && currentlyRunningTimerEntry != null && runningEntryExistsInDayEntries;

        if (runningEntryIsOnThisDate)
        {
            if (loadedWorkItems.TryGetValue(currentlyRunningTimerEntry.WorkItemId, out var freshWorkItem))
            {
                currentlyRunningTimerEntry.ProjectPicker.SelectedProject = freshWorkItem.Project;
            }

            AddDayEntry(currentlyRunningTimerEntry, 0);
        }
    }

    private async Task<WorkItem> ResolveWorkItem(IDictionary<Guid, WorkItem> workItems, Guid workItemId)
    {
        if (workItems.ContainsKey(workItemId))
        {
            return workItems[workItemId];
        }

        var filter = new WorkItemGetFilter()
        {
            WorkItemId = workItemId
        };

        var workItem = await workItemService.Get(filter);

        return workItem.Single();
    }

    // Event Handlers

    private void Timer_Tick(object sender, EventArgs e)
    {
        currentlyRunningTimerEntry?.AddDuration(TimeSpan.FromSeconds(1));
    }

    private void Recents_AddToMyDayRequested(object sender, WorkItem workItem)
    {
        _ = AddWorkItemToMyDay(workItem);
    }

    private void Recents_StartAndAddToMyDayRequested(object sender, WorkItem workItem)
    {
        _ = AddWorkItemToMyDay(workItem, startTimer: IsToday());
    }

    private async void DayEntry_Deleted(object sender, EventArgs e)
    {
        var viewModel = sender as DayEntryViewModel;

        if (currentlyRunningTimerEntry == viewModel)
        {
            timer.Stop();
            currentlyRunningTimerEntry = null;
            OnPropertyChanged(nameof(IsTimerRunning));
        }

        try
        {
            var recent = await recentWorkItemViewModelFactory.CreateRecentWorkItemViewModel(viewModel.WorkItem, IsToday());

            Recents.Add(recent);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }

        RemoveDayEntry(viewModel);
    }

    private void DayEntry_DurationChanged(object sender, DayEntryDurationChangedArgs e)
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

    private void StartTimerForEntry(DayEntryViewModel viewModel)
    {
        if (currentlyRunningTimerEntry != null && currentlyRunningTimerEntry != viewModel)
        {
            currentlyRunningTimerEntry.IsTimerRunning = false;
        }

        currentlyRunningTimerEntry = viewModel;
        currentlyRunningTimerEntry.IsTimerRunning = true;
        timer.Start();

        OnPropertyChanged(nameof(IsTimerRunning));
    }

    private void DayEntry_TimerStateChanged(object sender, TimerStateChangedArgs e)
    {
        var viewModel = sender as DayEntryViewModel;

        if (e.TimerStateChange == TimerStateChange.Start)
        {
            StartTimerForEntry(viewModel);
        }
        else if (e.TimerStateChange == TimerStateChange.Stop)
        {
            viewModel.IsTimerRunning = false;

            if (currentlyRunningTimerEntry == viewModel)
            {
                currentlyRunningTimerEntry = null;
                timer.Stop();

                OnPropertyChanged(nameof(IsTimerRunning));
            }
        }
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.SessionLock)
        {
            OnSessionLocked();
        }
        else if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            OnSessionUnlocked();
        }
    }

    private void OnSessionLocked()
    {
        if (currentlyRunningTimerEntry == null || !timer.IsEnabled) return;

        sessionLockTime = DateTime.Now;

        Application.Current.Dispatcher.Invoke(() =>
        {
            currentlyRunningTimerEntry.IsTimerRunning = false;
            timer.Stop();
            OnPropertyChanged(nameof(IsTimerRunning));
        });
    }

    private void OnSessionUnlocked()
    {
        if (sessionLockTime == null || currentlyRunningTimerEntry == null) return;

        var awayDuration = TimeSpan.FromSeconds(Math.Floor((DateTime.Now - sessionLockTime.Value).TotalSeconds));
        var entryName = currentlyRunningTimerEntry.Name;
        sessionLockTime = null;

        Application.Current.Dispatcher.Invoke(() =>
        {
            var formatted = awayDuration.ToString(@"hh\:mm\:ss");

            var dialog = new MyKhronus.WPF.Windows.ConfirmWindow(
                $"You were away for {formatted}.\n\nWould you like to add this time to \"{entryName}\"?",
                "Welcome back");

            if (dialog.ShowDialog() == true)
            {
                currentlyRunningTimerEntry.AddDuration(awayDuration);
            }
        });
    }

    private async void AsyncWrapper(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unhandled async error: {ex}");
        }
    }

    public void Dispose()
    {
        timer.Stop();
        timer.Tick -= Timer_Tick;

        autoSaveTimer.Stop();

        Recents.AddToMyDayRequested -= Recents_AddToMyDayRequested;
        Recents.StartAndAddToMyDayRequested -= Recents_StartAndAddToMyDayRequested;
        Recents.Dispose();

        SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
    }
}
