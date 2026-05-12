namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Windows;
using System.Windows.Input;

using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Utilities;

public class RecentWorkItemViewModel : NotifyPropertyChanged, IDisposable
{
    public event EventHandler Deleted;
    public event EventHandler AddedToMyDay;
    public event EventHandler StartedAndAddedToMyDay;

    private readonly IWorkItemService workItemService;
    private readonly bool canStartTimer;

    private WorkItem workItem;

    public RecentWorkItemViewModel(
        WorkItem workItem,
        IWorkItemService workItemService,
        ProjectPickerViewModel projectPicker,
        bool canStartTimer)
    {
        this.workItem = workItem;
        this.workItemService = workItemService;
        this.canStartTimer = canStartTimer;

        ProjectPicker = projectPicker;
        ProjectPicker.SelectedProject = workItem.Project;
        ProjectPicker.ProjectChanged += ProjectPicker_ProjectChanged;
    }

    public WorkItem WorkItem => workItem;

    public Guid WorkItemId => workItem.Id;

    public string Name => workItem.Description;

    private ProjectPickerViewModel projectPicker;

    public ProjectPickerViewModel ProjectPicker
    {
        get { return projectPicker; }
        private set
        {
            projectPicker = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddToMyDay => new RelayCommand(() => AddedToMyDay?.Invoke(this, EventArgs.Empty));

    public ICommand StartAddToMyDay => new RelayCommand(
        () => StartedAndAddedToMyDay?.Invoke(this, EventArgs.Empty),
        () => canStartTimer);

    public ICommand Delete => new RelayCommand(async () =>
    {
        await workItemService.Delete(WorkItemId);

        Deleted?.Invoke(this, EventArgs.Empty);
    });

    private async void ProjectPicker_ProjectChanged(object sender, EventArgs e)
    {
        var picker = sender as ProjectPickerViewModel;

        try
        {
            if (picker?.SelectedProject == null)
            {
                await workItemService.UnlinkWorkItemToProject(workItem.Id);
            }
            else
            {
                await workItemService.LinkWorkItemToProject(workItem.Id, picker.SelectedProject.Id);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    }

    public void Dispose()
    {
        projectPicker.ProjectChanged -= ProjectPicker_ProjectChanged;
    }
}
