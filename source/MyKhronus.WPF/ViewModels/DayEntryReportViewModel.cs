namespace MyKhronus.WPF.ViewModels;

using System.Windows;
using System.Windows.Input;

using MyKhronus.DataAccess.DayEntries.Models;
using MyKhronus.WPF.Utilities;

public class DayEntryReportViewModel
{
    public DayEntryReportViewModel(DateTime entryDate, TimeSpan duration)
    {
        RecordDate = entryDate.ToString("ddd dd/MM");

        Duration = duration.ToString(@"hh\:mm");

        HasEntry = true;
    }

    public DayEntryReportViewModel(DateTime date)
    {
        RecordDate = date.ToString("ddd dd/MM");

        Duration = string.Empty;

        HasEntry = false;
    }

    public string RecordDate { get; }

    public string Duration { get; }

    public bool HasEntry { get; }

    public ICommand Copy => new RelayCommand(() => Clipboard.SetText(Duration));
}
