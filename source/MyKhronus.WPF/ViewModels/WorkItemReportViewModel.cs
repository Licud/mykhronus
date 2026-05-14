namespace MyKhronus.WPF.ViewModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

using MyKhronus.DataAccess.DayEntries.Models;
using MyKhronus.DataAccess.Projects.Models;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.WPF.Utilities;

public class WorkItemReportViewModel : NotifyPropertyChanged
{
    private readonly IEnumerable<WorkItem> workItems;
    private readonly IReadOnlyList<DateTime> dateSequence;
    private readonly Project project;
    private readonly IReadOnlyDictionary<DateTime, IEnumerable<DayEntry>> entriesByDate;

    public WorkItemReportViewModel(
        IEnumerable<WorkItem> workItems,
        IEnumerable<DayEntry> entries,
        IReadOnlyList<DateTime> dateSequence,
        Project project = null)
    {
        this.workItems = workItems;

        this.dateSequence = dateSequence;
        this.project = project;
        this.entriesByDate = entries
            .GroupBy(g => g.EntryDate.Date)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        Records = new ObservableCollection<DayEntryReportViewModel>();

        SetDescription();
        SetRecords();
    }

    public string ProjectName => project?.Name ?? "No Project";

    private string description;

    public string Description
    {
        get { return description; }
        set
        {
            description = value;
            OnPropertyChanged();
        }
    }

    private bool isDisplayed = true;

    public bool IsDisplayed
    {
        get { return isDisplayed; }
        set
        {
            isDisplayed = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<DayEntryReportViewModel> Records { get; }

    private void SetDescription()
    {
        var stringBuilder = new StringBuilder();

        foreach (var workItem in workItems)
        {
            stringBuilder.AppendLine(workItem.Description);
        }

        Description = stringBuilder.ToString();
    }

    private void SetRecords()
    {
        foreach (var date in dateSequence)
        {
            if (entriesByDate.ContainsKey(date))
            {
                var entries = entriesByDate[date];

                var timespan = TimeSpan.Zero;

                foreach (var entry in entries)
                {
                    timespan += entry.Duration;
                }

                Records.Add(new DayEntryReportViewModel(date, timespan));
            }
            else
            {
                Records.Add(new DayEntryReportViewModel(date));
            }
        }
    }

    public ICommand Copy => new RelayCommand(() => Clipboard.SetText(Description));

    public ICommand Hide => new RelayCommand(() => IsDisplayed = false);
}
