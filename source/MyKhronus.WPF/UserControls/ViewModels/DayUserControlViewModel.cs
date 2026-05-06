namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Utilities;

public class DayUserControlViewModel : MainViewModelControls
{
    private readonly IWorkItemService workItemService;
    private readonly ObservableCollection<RecentWorkItemViewModel> recentWorkItems = [];

    public DayUserControlViewModel(IWorkItemService workItemService)
    {
        this.workItemService = workItemService;

        var savedWorkItems = Task.Run(() => workItemService.Get(new())).GetAwaiter().GetResult();

        foreach (var workItem in savedWorkItems)
        {
            AddRecentWorkItem(new RecentWorkItemViewModel(workItem, workItemService));
        }

        RecentWorkItems = CollectionViewSource.GetDefaultView(recentWorkItems);
        RecentWorkItems.Filter = new Predicate<object>(WorkItemNameContains);
    }

    public ICollectionView RecentWorkItems { get; }

    public bool HasRecentWorkItems => recentWorkItems.Count > 0;

    private string workItemFilter = "";

    public string WorkItemFilter
    {
        get { return workItemFilter; }
        set
        {
            workItemFilter = value;
            RecentWorkItems.Refresh();
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
            await AddWorkItem();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    });

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

        recentWorkItems.Add(viewModel);
    }

    private void RecentWorkItem_Deleted(object sender, EventArgs e)
    {
        var viewModel = sender as RecentWorkItemViewModel;

        viewModel.Deleted -= RecentWorkItem_Deleted;

        recentWorkItems.Remove(viewModel);
    }

    private bool WorkItemNameContains(object viewModel)
    {
        var recentWorkItemViewModel = viewModel as RecentWorkItemViewModel;

        return recentWorkItemViewModel?.Name.Contains(WorkItemFilter, StringComparison.InvariantCultureIgnoreCase) == true;
    }
}
