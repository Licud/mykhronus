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

        var workItems = await workItemService.Get(new WorkItemGetFilter());

        var workItemsById = workItems.ToDictionary(w => w.Id);

        var dateSequence = BuildDateSequence(From.Date, To.Date);

        var entriesByWorkItem = entries
            .GroupBy(e => e.WorkItemId)
            .Where(g => workItemsById.ContainsKey(g.Key))
            .ToList();

        var reportsByDate = new Dictionary<DateTime, List<WorkItemReportViewModel>>();

        foreach (var group in entriesByWorkItem)
        {
            var workItem = workItemsById[group.Key];

            var groupedEntries = group.ToList();

            var firstEntry = groupedEntries.OrderBy(e => e.EntryDate).First();

            var viewModel = new WorkItemReportViewModel(workItem, groupedEntries, dateSequence);

            if (!reportsByDate.ContainsKey(firstEntry.EntryDate.Date))
            {
                reportsByDate.Add(firstEntry.EntryDate.Date, new List<WorkItemReportViewModel>());
            }

            reportsByDate[firstEntry.EntryDate.Date].Add(viewModel);
        }

        foreach (var key in reportsByDate.Keys.OrderBy(k => k))
        {
            foreach (var viewModel in reportsByDate[key])
            {
                WorkItemReports.Add(viewModel);
            }
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
