namespace MyKhronus.WPF.UIModels;

using System.Collections.Generic;

using MyKhronus.DataAccess.Services;
using MyKhronus.Models;
using MyKhronus.WPF.Extenstions;
using MyKhronus.WPF.Services;
using MyKhronus.WPF.ViewModels;

public class ActivityRecordCollection : ObservableViewModelCollection<ActivityRecord, ActivityRecordViewModel>
{
    private readonly IActivityRecordsService activityRecordsService;
    private readonly IActivityRecordTimerService activityRecordTimerService;

    public ActivityRecordCollection(IActivityRecordsService activityRecordsService,
        IActivityRecordTimerService activityRecordTimerService)
    {
        this.activityRecordsService = activityRecordsService;
        this.activityRecordTimerService = activityRecordTimerService;
    }

    public override void LoadItems(IEnumerable<ActivityRecord> models)
    {
        foreach (var model in models)
        {
            Add(model);
        }
    }

    public override void Add(ActivityRecord model)
    {
        var viewModel = model
            .ToViewModel(activityRecordsService, activityRecordTimerService);

        Add(viewModel);
    }

    public override void Remove(ActivityRecord model)
    {
        var itemToRemove = this.FirstOrDefault(i => i.ActivityRecordId == model.ActivityRecordId);

        Remove(itemToRemove);
    }

    protected override void ClearItems()
    {
        var tempItems = this.ToList();

        foreach (var tempItem in tempItems)
        {
            var activityRecord = tempItem.ActivityRecord;

            Remove(activityRecord);
        }
    }
}
