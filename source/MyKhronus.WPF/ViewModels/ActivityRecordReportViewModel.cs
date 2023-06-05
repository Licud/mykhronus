namespace MyKhronus.WPF.ViewModels;

using System.Windows;
using System.Windows.Input;

using MyKhronus.Models;
using MyKhronus.WPF.Utilities;

public class ActivityRecordReportViewModel
{
    public ActivityRecordReportViewModel(ActivityRecord activityRecord)
    {
        RecordDate = activityRecord.RecordDateOnly.ToString("ddd dd/MM");

        Duration = activityRecord.Duration.ToString(@"hh\:mm");
    }

    public string RecordDate { get; }

    public string Duration { get; }

    public ICommand Copy => new RelayCommand(() => Clipboard.SetText(Duration));
}
