namespace MyKhronus.WPF.ViewModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

using MyKhronus.Commons.Utilities;
using MyKhronus.Models;
using MyKhronus.WPF.Utilities;

public class ActivityReportViewModel : NotifyPropertyChanged
{
    private readonly Activity activity;
    private readonly IEnumerable<ActivityRecord> records;

    public ActivityReportViewModel(Activity activity, IEnumerable<ActivityRecord> records)
    {
        this.activity = activity;

        this.records = records.OrderBy(r => r.RecordDate);

        Records = new ObservableCollection<ActivityRecordReportViewModel>();

        SetDescription();
        SetRecords();
    }

    public string Name => activity.Name;

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

    public ObservableCollection<ActivityRecordReportViewModel> Records { get; }

    private void SetDescription()
    {
        var descriptionBuilder = new StringBuilder();

        foreach (var record in records)
        {
            descriptionBuilder.AppendLine(record.Description);
        }

        Description = descriptionBuilder.ToString();
    }

    private void SetRecords()
    {
        foreach (var record in records)
        {
            Records.Add(new ActivityRecordReportViewModel(record));
        }
    }
    public ICommand Copy => new RelayCommand(() => Clipboard.SetText(Description));

    public ICommand Hide => new RelayCommand(() => IsDisplayed = false);
}
