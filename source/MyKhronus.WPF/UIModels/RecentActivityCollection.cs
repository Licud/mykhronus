namespace MyKhronus.WPF.UIModels;

using System.Collections.Generic;

using MyKhronus.DataAccess.Services;
using MyKhronus.Models;
using MyKhronus.WPF.Extenstions;
using MyKhronus.WPF.Services;
using MyKhronus.WPF.UserControls.ViewModels;

public class RecentActivityCollection : ObservableViewModelCollection<Activity, RecentActivityViewModel>
{
    private readonly ActivityRecordsService activityRecordsService;
    private readonly IActivityRecordTimerService activityRecordTimerService;

    public RecentActivityCollection(ActivityRecordsService activityRecordsService,
        IActivityRecordTimerService activityRecordTimerService)
    {
        this.activityRecordsService = activityRecordsService;
        this.activityRecordTimerService = activityRecordTimerService;
    }

    public override void LoadItems(IEnumerable<Activity> models)
    {
        foreach (var model in models)
        {
            Add(model);
        }
    }

    public override void Add(Activity model)
    {
        var viewModel = model.ToViewModel(activityRecordsService, activityRecordTimerService);

        Add(viewModel);
    }

    public override void Remove(Activity model)
    {
        var itemToRemove = this.FirstOrDefault(i => i.ActivityId == model.ActivityId);

        Remove(itemToRemove);
    }
}
