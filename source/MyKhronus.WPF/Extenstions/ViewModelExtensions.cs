namespace MyKhronus.WPF.Extenstions;

using MyKhronus.DataAccess.Services;
using MyKhronus.Models;
using MyKhronus.WPF.Services;
using MyKhronus.WPF.UserControls.ViewModels;
using MyKhronus.WPF.ViewModels;

internal static class ViewModelExtensions
{
    internal static ActivityRecordViewModel ToViewModel(this ActivityRecord activityRecord,
        IActivityRecordsService activityRecordsService,
        IActivityRecordTimerService activityRecordTimerService)
        => new ActivityRecordViewModel(activityRecord, activityRecordsService, activityRecordTimerService);

    internal static RecentActivityViewModel ToViewModel(this Activity activity, 
        IActivityRecordsService activityRecordsService,
        IActivityRecordTimerService activityRecordTimerService)
        => new RecentActivityViewModel(activity, activityRecordsService, activityRecordTimerService);
}
