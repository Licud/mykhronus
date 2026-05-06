namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Windows.Input;

using MyKhronus.Commons.Utilities;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Utilities;

public class RecentWorkItemViewModel(WorkItem workItem, IWorkItemService workItemService) : NotifyPropertyChanged
{
    public event EventHandler Deleted;

    public Guid WorkItemId => workItem.Id;

    public string Name => workItem.Description;

    public ICommand AddToMyDay => new RelayCommand(ExecuteAddToMyDay);

    private void ExecuteAddToMyDay()
    {
        // TODO: add work item to day entries
    }

    public ICommand StartAddToMyDay => new RelayCommand(ExecuteStartAddToMyDay);

    private void ExecuteStartAddToMyDay()
    {
        // TODO: add work item to day entries and start timer
    }

    public ICommand Delete => new RelayCommand(async () => 
    {
        await workItemService.Delete(WorkItemId);
        Deleted?.Invoke(this, EventArgs.Empty);
    });
}
