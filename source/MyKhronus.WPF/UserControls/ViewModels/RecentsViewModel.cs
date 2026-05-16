namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Builders;
using MyKhronus.WPF.Utilities;

public class RecentsViewModel : NotifyPropertyChanged, IDisposable
{
    public event EventHandler<WorkItem>? AddToMyDayRequested;
    public event EventHandler<WorkItem>? StartAndAddToMyDayRequested;

    private readonly IWorkItemService workItemService;
    private readonly RecentWorkItemViewModelFactory recentWorkItemViewModelFactory;
    private readonly ObservableCollection<RecentWorkItemViewModel> items = [];
    private readonly DispatcherTimer searchDebounce = new() { Interval = TimeSpan.FromMilliseconds(250) };

    private string filter = "";
    private const int MinFilterSearchLength = 2;

    public bool IsToday { get; set; } = true;

    public Func<Guid, bool>? IsAlreadyInMyDay { get; set; }

    public RecentsViewModel(
        IWorkItemService workItemService,
        RecentWorkItemViewModelFactory recentWorkItemViewModelFactory)
    {
        this.workItemService = workItemService;
        this.recentWorkItemViewModelFactory = recentWorkItemViewModelFactory;

        Items = CollectionViewSource.GetDefaultView(items);
        Items.Filter = obj => (obj as RecentWorkItemViewModel)
            ?.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) == true;

        items.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasItems));

        searchDebounce.Tick += async (_, _) => await ExecuteSearch();
    }

    public ICollectionView Items { get; }

    public bool HasItems => items.Count > 0;

    public void SetFilter(string value)
    {
        filter = value;
        Items.Refresh();
        ScheduleSearch();
    }

    public void Add(RecentWorkItemViewModel viewModel)
    {
        viewModel.Deleted += Item_Deleted;
        viewModel.AddedToMyDay += Item_AddedToMyDay;
        viewModel.StartedAndAddedToMyDay += Item_StartedAndAddedToMyDay;

        items.Add(viewModel);
    }

    public void Remove(RecentWorkItemViewModel viewModel)
    {
        viewModel.Deleted -= Item_Deleted;
        viewModel.AddedToMyDay -= Item_AddedToMyDay;
        viewModel.StartedAndAddedToMyDay -= Item_StartedAndAddedToMyDay;
        viewModel.Dispose();

        items.Remove(viewModel);
    }

    public void Clear()
    {
        foreach (var item in items.ToList())
        {
            Remove(item);
        }
    }

    public bool Contains(Guid workItemId) => items.Any(r => r.WorkItemId == workItemId);

    public RecentWorkItemViewModel? FindById(Guid workItemId) =>
        items.FirstOrDefault(r => r.WorkItemId == workItemId);

    private void ScheduleSearch()
    {
        searchDebounce.Stop();

        if (string.IsNullOrWhiteSpace(filter) || filter.Length < MinFilterSearchLength)
        {
            return;
        }

        searchDebounce.Start();
    }

    private async Task ExecuteSearch()
    {
        searchDebounce.Stop();

        var term = filter;

        if (string.IsNullOrWhiteSpace(term) || term.Length < MinFilterSearchLength)
        {
            return;
        }

        try
        {
            var matches = await workItemService.Search(term);

            if (!string.Equals(term, filter, StringComparison.Ordinal))
            {
                return;
            }

            var added = false;

            foreach (var workItem in matches)
            {
                if (items.Any(r => r.WorkItemId == workItem.Id))
                {
                    continue;
                }

                if (IsAlreadyInMyDay?.Invoke(workItem.Id) == true)
                {
                    continue;
                }

                var recent = await recentWorkItemViewModelFactory.CreateRecentWorkItemViewModel(workItem, IsToday);
                Add(recent);
                added = true;
            }

            if (added)
            {
                Items.Refresh();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    }

    private void Item_Deleted(object? sender, EventArgs e)
    {
        if (sender is RecentWorkItemViewModel vm)
        {
            Remove(vm);
        }
    }

    private void Item_AddedToMyDay(object? sender, EventArgs e)
    {
        if (sender is RecentWorkItemViewModel vm)
        {
            AddToMyDayRequested?.Invoke(this, vm.WorkItem);
        }
    }

    private void Item_StartedAndAddedToMyDay(object? sender, EventArgs e)
    {
        if (sender is RecentWorkItemViewModel vm)
        {
            StartAndAddToMyDayRequested?.Invoke(this, vm.WorkItem);
        }
    }

    public void Dispose()
    {
        searchDebounce.Stop();
        Clear();
    }
}
