namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using MyKhronus.DataAccess.Services;
using MyKhronus.WPF.Extenstions;
using MyKhronus.WPF.Utilities;
using MyKhronus.WPF.ViewModels;

public class ReportsUserControlViewModel : MainViewModelControls
{
    private readonly IActivityService activityService;

    public ReportsUserControlViewModel(IActivityService activityService)
    {
        this.activityService = activityService;

        ActivityReports = new ObservableCollection<ActivityReportViewModel>();

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

    public ObservableCollection<ActivityReportViewModel> ActivityReports { get; }

    public ICommand Report => new RelayCommand(ExecuteReport, () => To.Date > From.Date);

    private void ExecuteReport()
    {
        ActivityReports.Clear();

        var activityRecords = activityService
            .GetRecordsBetween(From, To)
            .GroupBy(ar => ar.Activity.ActivityId)
            .Select(a => new { ActivityId = a.Key, Records = a.Select(ai => ai) })
            .ToList();

        var activityDictionary = new Dictionary<DateTime, List<ActivityReportViewModel>>();

        foreach (var activityRecord in activityRecords)
        {
            var firstRecord = activityRecord
                .Records
                .OrderBy(r => r.RecordDate.Date)
                .First();

            var viewModel = (new ActivityReportViewModel(firstRecord.Activity, activityRecord.Records));

            if (!activityDictionary.ContainsKey(firstRecord.RecordDate.Date))
            {
                var newViewModelList = new List<ActivityReportViewModel>();

                activityDictionary.Add(firstRecord.RecordDate.Date, newViewModelList);
            }

            activityDictionary[firstRecord.RecordDate.Date].Add(viewModel);
        }

        foreach (var key in activityDictionary.Keys.OrderBy(k => k))
        {
            foreach (var viewModel in activityDictionary[key])
            {
                ActivityReports.Add(viewModel);
            }
        }
    }
}
