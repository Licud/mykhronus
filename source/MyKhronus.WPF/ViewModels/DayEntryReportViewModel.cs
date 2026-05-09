namespace MyKhronus.WPF.ViewModels;

using System.Windows;
using System.Windows.Input;

using MyKhronus.DataAccess.DayEntries.Models;
using MyKhronus.WPF.Utilities;

public class DayEntryReportViewModel
{
    public DayEntryReportViewModel(DayEntry dayEntry)
    {
        RecordDate = dayEntry.EntryDate.ToString("ddd dd/MM");

        Duration = dayEntry.Duration.ToString(@"hh\:mm");
    }

    public string RecordDate { get; }

    public string Duration { get; }

    public ICommand Copy => new RelayCommand(() => Clipboard.SetText(Duration));
}
