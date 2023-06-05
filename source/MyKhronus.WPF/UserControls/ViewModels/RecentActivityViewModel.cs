namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Windows.Input;

using MyKhronus.Commons.Utilities;
using MyKhronus.DataAccess.Services;
using MyKhronus.Models;
using MyKhronus.WPF.Services;
using MyKhronus.WPF.Utilities;

public class RecentActivityViewModel : NotifyPropertyChanged
{
    private readonly IActivityRecordsService activityRecordService;
    private readonly IActivityRecordTimerService activityRecordTimerService;

    public RecentActivityViewModel(Activity activity,
        IActivityRecordsService activityRecordService,
        IActivityRecordTimerService activityRecordTimerService)
    {
        Activity = activity;

        this.activityRecordService = activityRecordService;
        this.activityRecordTimerService = activityRecordTimerService;
    }

    public Activity Activity { get; }

    public Guid ActivityId => Activity.ActivityId;

    public string Name => Activity.Name;

    public ICommand AddToMyDay => new RelayCommand(ExecuteAddToMyDay);

    private void ExecuteAddToMyDay() => activityRecordService.AddToMyDay(Activity);

    public ICommand StartAddToMyDay => new RelayCommand(ExecuteStartAddToMyDay, CanExecuteStartAddToMyDay);

    private void ExecuteStartAddToMyDay() 
    {
        var activityRecord = activityRecordService.AddToMyDay(Activity);

        activityRecordTimerService.StartTimer(activityRecord);
    }

    private bool CanExecuteStartAddToMyDay() => DateTime.Today.Date == activityRecordService.SelectedDate.Date;

    public ICommand Delete => new RelayCommand(ExecuteDelete);

    private void ExecuteDelete() => activityRecordService.DeleteActivity(Activity);

}
