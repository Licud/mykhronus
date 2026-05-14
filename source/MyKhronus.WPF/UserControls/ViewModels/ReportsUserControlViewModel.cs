namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using MyKhronus.DataAccess.DayEntries.Services;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Extenstions;
using MyKhronus.WPF.Utilities;
using MyKhronus.WPF.ViewModels;

public class ReportsUserControlViewModel : MainViewModelControls
{
    private readonly IDailyEntryService dailyEntryService;
    private readonly IWorkItemService workItemService;

    public ReportsUserControlViewModel(
        IDailyEntryService dailyEntryService,
        IWorkItemService workItemService)
    {
        this.dailyEntryService = dailyEntryService;
        this.workItemService = workItemService;

        WorkItemReports = new ObservableCollection<WorkItemReportViewModel>();

        From = DateTime.Today.GetMondayDateOfWeek();
        To = From.AddDays(4);
    }

    private DateTime from;

    public DateTime From
    {
        get { return from; }
        set
        {
            from = value;
            OnPropertyChanged();
        }
    }

    private DateTime to;

    public DateTime To
    {
        get { return to; }
        set
        {
            to = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<WorkItemReportViewModel> WorkItemReports { get; }

    public ICommand Report => new RelayCommand(async () => await ExecuteReport(), () => To.Date > From.Date);

    private async Task ExecuteReport()
    {
        WorkItemReports.Clear();

        var entries = await dailyEntryService.GetEntriesBetween(From, To);

        var workItems = await workItemService.Get(new WorkItemGetFilter()
        {
            WorkItemIds = entries.Select(e => e.WorkItemId).ToList(),
        });

        var groupedByProject = workItems
            .Where(w => w.Project != null)
            .GroupBy(w => w.Project);

        var dateSequence = BuildDateSequence(From.Date, To.Date);

        foreach (var projectGroup in groupedByProject)
        {
            var project = projectGroup.Key;

            var workItemsInProject = projectGroup.ToList();

            var workItemIds = workItemsInProject.Select(w => w.Id).ToHashSet();

            var workItemEntries = entries.Where(e => workItemIds.Contains(e.WorkItemId));

            var viewModel = new WorkItemReportViewModel(workItemsInProject, workItemEntries, dateSequence, project);

            WorkItemReports.Add(viewModel);
        }

        var workItemsWithoutProject = workItems
            .Where(w => w.Project == null)
            .ToList();

        foreach (var item in workItemsWithoutProject)
        {
            var workItemEntries = entries.Where(e => e.WorkItemId == item.Id);

            var viewModel = new WorkItemReportViewModel([item], workItemEntries, dateSequence, null);

            WorkItemReports.Add(viewModel);
        }
    }

    private static IReadOnlyList<DateTime> BuildDateSequence(DateTime from, DateTime to)
    {
        var dates = new List<DateTime>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            dates.Add(date);
        }

        return dates;
    }
}
