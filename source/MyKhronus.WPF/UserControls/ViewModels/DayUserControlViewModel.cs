namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using System.Timers;

using MyKhronus.DataAccess.DayEntries.Services;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.Models.Enums;
using MyKhronus.WPF.UserControls.EventArguments;
using MyKhronus.WPF.Utilities;

public class DayUserControlViewModel : MainViewModelControls
{
    private const int intervalInMilliseconds = 1000;

    private readonly IWorkItemService workItemService;
    private readonly IDailyEntryService dailyEntryService;
    private readonly ObservableCollection<RecentWorkItemViewModel> recentWorkItems = [];
    private readonly ObservableCollection<DayEntryViewModel> myDayEntries = [];

    private Timer timer = new Timer(intervalInMilliseconds);
    private DayEntryViewModel currentlyRunningTimerEntry;

    public DayUserControlViewModel(IWorkItemService workItemService, IDailyEntryService dailyEntryService)
    {
        this.workItemService = workItemService;
        this.dailyEntryService = dailyEntryService;

        RecentWorkItems = CollectionViewSource.GetDefaultView(recentWorkItems);
        RecentWorkItems.Filter = new Predicate<object>(WorkItemNameContains);

        MyDay = CollectionViewSource.GetDefaultView(myDayEntries);
        MyDay.Filter = new Predicate<object>(DayEntryNameContains);

        selectedDate = DateTime.Today;

        ReloadCollections();

        timer.Elapsed += Timer_Elapsed;
    }

    // Properties

    public ICollectionView RecentWorkItems { get; }

    public ICollectionView MyDay { get; }

    public bool HasRecentWorkItems => recentWorkItems.Count > 0;

    private string workItemFilter = "";

    public string WorkItemFilter
    {
        get { return workItemFilter; }
        set
        {
            workItemFilter = value;
            RecentWorkItems.Refresh();
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
                ReloadCollections();
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

    public string TotalDurationDisplay => TotalDuration.ToString();

    public ICommand AddNewEntry => new RelayCommand(async () => await ExecuteAddNewEntry(), CanAddNewEntry);
        
    public ICommand AddAndStartNewEntry => new RelayCommand(async () => await ExecuteAddAndStartNewEntry(), CanAddNewEntry);

    public ICommand PreviousDay => new RelayCommand(() => SelectedDate = SelectedDate.AddDays(-1));

    public ICommand NextDay => new RelayCommand(() => SelectedDate = SelectedDate.AddDays(1),
        () => SelectedDate.Date != DateTime.Today.Date);

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

    private bool CanAddNewEntry()
    {
        return !string.IsNullOrWhiteSpace(NewEntryName);
    }

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
        }

        var inRecentWorkItems = recentWorkItems.Any(r => r.WorkItemId == savedWorkItem.Id);
        var inMyDayEntries = myDayEntries.Any(e => e.WorkItemId == savedWorkItem.Id);

        if (!inRecentWorkItems && !inMyDayEntries)
        {
            AddRecentWorkItem(new RecentWorkItemViewModel(savedWorkItem, workItemService));
        }
        else
        {
            WorkItemFilter = newName;
        }

        NewEntryName = string.Empty;

        return savedWorkItem;
    }

    private void AddRecentWorkItem(RecentWorkItemViewModel viewModel)
    {
        viewModel.Deleted += RecentWorkItem_Deleted;
        viewModel.AddedToMyDay += RecentWorkItem_AddedToMyDay;
        viewModel.StartedAndAddedToMyDay += RecentWorkItem_StartedAndAddedToMyDay;

        recentWorkItems.Add(viewModel);
    }

    private void RemoveRecentWorkItem(RecentWorkItemViewModel viewModel)
    {
        viewModel.Deleted -= RecentWorkItem_Deleted;
        viewModel.AddedToMyDay -= RecentWorkItem_AddedToMyDay;
        viewModel.StartedAndAddedToMyDay -= RecentWorkItem_StartedAndAddedToMyDay;

        recentWorkItems.Remove(viewModel);
    }

    private bool WorkItemNameContains(object viewModel)
    {
        var recentWorkItemViewModel = viewModel as RecentWorkItemViewModel;

        return recentWorkItemViewModel?.Name.Contains(WorkItemFilter, StringComparison.InvariantCultureIgnoreCase) == true;
    }

    private bool DayEntryNameContains(object viewModel)
    {
        var dayEntryViewModel = viewModel as DayEntryViewModel;

        return dayEntryViewModel?.Name.Contains(WorkItemFilter, StringComparison.InvariantCultureIgnoreCase) == true;
    }

    // Day Entry Methods

    private async Task AddWorkItemToMyDay(WorkItem workItem, bool startTimer = false)
    {
        var currentDayEntry = myDayEntries.FirstOrDefault(e => e.WorkItemId == workItem.Id);

        if (currentDayEntry == null)
        {
            var dayEntry = await dailyEntryService.Add(selectedDate, workItem.Id);

            currentDayEntry = new DayEntryViewModel(dayEntry, workItem, dailyEntryService);

            AddDayEntry(currentDayEntry);
            RemoveRecentWorkItem(recentWorkItems.FirstOrDefault(r => r.WorkItemId == workItem.Id));
        }

        if (startTimer)
        {
            StartTimer(currentDayEntry);
        }
    }

    private void AddDayEntry(DayEntryViewModel viewModel)
    {
        viewModel.Deleted += DayEntry_Deleted;
        viewModel.DurationChanged += DayEntry_DurationChanged;
        viewModel.TimerStateChanged += DayEntry_TimerStateChanged;

        myDayEntries.Add(viewModel);

        TotalDuration = TotalDuration.Add(viewModel.Duration);
    }

    private void RemoveDayEntry(DayEntryViewModel viewModel)
    {
        viewModel.Deleted -= DayEntry_Deleted;
        viewModel.DurationChanged -= DayEntry_DurationChanged;
        viewModel.TimerStateChanged -= DayEntry_TimerStateChanged;

        if (currentlyRunningTimerEntry == viewModel)
        {
            currentlyRunningTimerEntry = null;
        }

        myDayEntries.Remove(viewModel);

        TotalDuration = TotalDuration.Subtract(viewModel.Duration);
    }

    private void ReloadCollections()
    {
        foreach (var item in recentWorkItems.ToList())
        {
            RemoveRecentWorkItem(item);
        }

        foreach (var entry in myDayEntries.ToList())
        {
            RemoveDayEntry(entry);
        }

        TotalDuration = TimeSpan.Zero;

        var workItems = Task.Run(() => workItemService.Get(new())).GetAwaiter().GetResult();
        var dayEntries = Task.Run(() => dailyEntryService.GetEntries(selectedDate)).GetAwaiter().GetResult();

        var dayEntryWorkItemIds = dayEntries.Select(e => e.WorkItemId).ToHashSet();

        var selectableWorkItems = workItems.Where(w => !dayEntryWorkItemIds.Contains(w.Id)).ToList();

        foreach (var item in selectableWorkItems)
        {
            AddRecentWorkItem(new RecentWorkItemViewModel(item, workItemService));
        }

        var loadedWorkItems = workItems.ToDictionary(w => w.Id, w => w);

        foreach (var dayEntry in dayEntries)
        {
            var workItem = Task.Run(() => ResolveWorkItem(loadedWorkItems, dayEntry.WorkItemId))
                .GetAwaiter()
                .GetResult();

            var viewModel = new DayEntryViewModel(dayEntry, workItem, dailyEntryService);

            AddDayEntry(viewModel);
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

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        currentlyRunningTimerEntry?.AddDuration(TimeSpan.FromSeconds(1));
    }

    private void RecentWorkItem_Deleted(object sender, EventArgs e)
    {
        var viewModel = sender as RecentWorkItemViewModel;

        RemoveRecentWorkItem(viewModel);
    }

    private void RecentWorkItem_AddedToMyDay(object sender, EventArgs e)
    {
        var viewModel = sender as RecentWorkItemViewModel;

        _ = AddWorkItemToMyDay(viewModel.WorkItem);
    }

    private void RecentWorkItem_StartedAndAddedToMyDay(object sender, EventArgs e)
    {
        var viewModel = sender as RecentWorkItemViewModel;

        _ = AddWorkItemToMyDay(viewModel.WorkItem, startTimer: true);
    }

    private void DayEntry_Deleted(object sender, EventArgs e)
    {
        var viewModel = sender as DayEntryViewModel;

        AddRecentWorkItem(new RecentWorkItemViewModel(viewModel.WorkItem, workItemService));

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

    private void StartTimer(DayEntryViewModel viewModel)
    {
        if (currentlyRunningTimerEntry != null)
        {
            currentlyRunningTimerEntry.IsTimerRunning = false;
        }

        currentlyRunningTimerEntry = viewModel;
        currentlyRunningTimerEntry.IsTimerRunning = true;
        timer.Start();
    }

    private void DayEntry_TimerStateChanged(object sender, TimerStateChangedArgs e)
    {
        var viewModel = sender as DayEntryViewModel;

        if (e.TimerStateChange == TimerStateChange.Start)
        {
            StartTimer(viewModel);
        }
        else if (e.TimerStateChange == TimerStateChange.Stop)
        {
            currentlyRunningTimerEntry.IsTimerRunning = false;
            currentlyRunningTimerEntry = null;
            timer.Stop();
        }
    }
}
