namespace MyKhronus.WPF.Builders;

using System.Threading.Tasks;

using MyKhronus.DataAccess.Projects.Services;
using MyKhronus.WPF.UserControls.ViewModels;

public class ProjectPickerViewModelFactory(IProjectService projectService)
{
    public async Task<ProjectPickerViewModel> CreateProjectPickerViewModel()
    {
        var projects = await projectService.Get();

        var projectPicker = new ProjectPickerViewModel(projectService, projects);

        return projectPicker;
    }
}
