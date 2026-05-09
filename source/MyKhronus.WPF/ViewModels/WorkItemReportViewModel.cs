namespace MyKhronus.WPF.ViewModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using MyKhronus.Commons.Utilities;
using MyKhronus.DataAccess.DayEntries.Models;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.WPF.Utilities;

public class WorkItemReportViewModel : NotifyPropertyChanged
{
    private readonly WorkItem workItem;
    private readonly IEnumerable<DayEntry> entries;

    public WorkItemReportViewModel(WorkItem workItem, IEnumerable<DayEntry> entries)
    {
        this.workItem = workItem;

        this.entries = entries.OrderBy(e => e.EntryDate);

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
        foreach (var entry in entries)
        {
            Records.Add(new DayEntryReportViewModel(entry));
        }
    }

    public ICommand Copy => new RelayCommand(() => Clipboard.SetText(Description));

    public ICommand Hide => new RelayCommand(() => IsDisplayed = false);
}
