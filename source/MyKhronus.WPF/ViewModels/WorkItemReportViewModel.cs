namespace MyKhronus.WPF.ViewModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using MyKhronus.DataAccess.DayEntries.Models;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.WPF.Utilities;

public class WorkItemReportViewModel : NotifyPropertyChanged
{
    private readonly WorkItem workItem;
    private readonly IReadOnlyList<DateTime> dateSequence;
    private readonly IReadOnlyDictionary<DateTime, DayEntry> entriesByDate;

    public WorkItemReportViewModel(
        WorkItem workItem,
        IEnumerable<DayEntry> entries,
        IReadOnlyList<DateTime> dateSequence)
    {
        this.workItem = workItem;

        this.dateSequence = dateSequence;

        this.entriesByDate = entries.ToDictionary(e => e.EntryDate.Date);

        Records = new ObservableCollection<DayEntryReportViewModel>();

        SetDescription();
        SetRecords();
    }

    public string Name => workItem.Description;

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
        Description = workItem.Description;
    }

    private void SetRecords()
    {
        foreach (var date in dateSequence)
        {
            if (entriesByDate.TryGetValue(date, out var entry))
            {
                Records.Add(new DayEntryReportViewModel(entry));
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
