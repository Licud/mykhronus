namespace MyKhronus.WPF.Builders;

using System.Threading.Tasks;

using MyKhronus.DataAccess.DayEntries.Models;
using MyKhronus.DataAccess.DayEntries.Services;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.UserControls.ViewModels;

public class DayEntryViewModelFactory(
    IDailyEntryService dailyEntryService,
    IWorkItemService workItemService,
    ProjectPickerViewModelFactory projectPickerViewModelFactory)
{
    public async Task<DayEntryViewModel> CreateDayEntryViewModel(DayEntry dayEntry, WorkItem workItem)
    {
        var projectPicker = await projectPickerViewModelFactory.CreateProjectPickerViewModel();

        return new DayEntryViewModel(dayEntry, workItem, dailyEntryService, workItemService, projectPicker);
    }
}
