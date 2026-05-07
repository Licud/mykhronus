namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using MyKhronus.DataAccess.DayEntries.Services;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Utilities;

public class DayUserControlViewModel : MainViewModelControls
{
    private readonly IWorkItemService workItemService;
    private readonly IDailyEntryService dailyEntryService;
    private readonly ObservableCollection<RecentWorkItemViewModel> recentWorkItems = [];
    private readonly ObservableCollection<DayEntryViewModel> myDayEntries = [];

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

    public ICommand AddNewEntry => new RelayCommand(async () =>
    {
        try
        {
            await AddWorkItem();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    });

    public ICommand AddAndStartNewEntry => new RelayCommand(async () =>
    {
        try
        {
            var entry = await AddWorkItem();

            await AddWorkItemToMyDay(entry);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    });

    public ICommand PreviousDay => new RelayCommand(() => SelectedDate = SelectedDate.AddDays(-1));

    public ICommand NextDay => new RelayCommand(() => SelectedDate = SelectedDate.AddDays(1),
        () => SelectedDate.Date != DateTime.Today.Date);

    // Recent Work Item Methods

    private async Task<WorkItem> AddWorkItem()
    {
        var newWorkItem = new NewWorkItem(NewEntryName.Trim());

        var addedWorkItem = await workItemService.Add(newWorkItem);

        if (!recentWorkItems.Select(r => r.WorkItemId).Contains(addedWorkItem.Id))
        {
            AddRecentWorkItem(new RecentWorkItemViewModel(addedWorkItem, workItemService));
        }

        NewEntryName = string.Empty;

        return addedWorkItem;
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
        if (myDayEntries.Any(e => e.WorkItemId == workItem.Id))
        {
            return;
        }

        var dayEntry = await dailyEntryService.Add(selectedDate, workItem.Id);

        var viewModel = new DayEntryViewModel(dayEntry, workItem, dailyEntryService);

        AddDayEntry(viewModel);
        RemoveRecentWorkItem(recentWorkItems.FirstOrDefault(r => r.WorkItemId == workItem.Id));

        if (startTimer)
        {
            // TODO: start timer for viewModel
        }
    }

    private void AddDayEntry(DayEntryViewModel viewModel)
    {
        viewModel.Deleted += DayEntry_Deleted;

        myDayEntries.Add(viewModel);

        TotalDuration = TotalDuration.Add(viewModel.Duration);
    }

    private void RemoveDayEntry(DayEntryViewModel viewModel)
    {
        viewModel.Deleted -= DayEntry_Deleted;

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
}
