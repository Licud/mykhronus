namespace MyKhronus.WPF.Builders;

using System.Threading.Tasks;

using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.UserControls.ViewModels;

public class RecentWorkItemViewModelFactory(
    IWorkItemService workItemService,
    ProjectPickerViewModelFactory projectPickerViewModelFactory)
{
    public async Task<RecentWorkItemViewModel> CreateRecentWorkItemViewModel(WorkItem workItem, bool canStartTimer)
    {
        var projectPicker = await projectPickerViewModelFactory.CreateProjectPickerViewModel();

        return new RecentWorkItemViewModel(workItem, workItemService, projectPicker, canStartTimer);
    }
}
