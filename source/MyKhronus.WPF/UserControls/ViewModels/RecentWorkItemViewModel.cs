namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Windows.Input;

using MyKhronus.Commons.Utilities;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Utilities;

public class RecentWorkItemViewModel(WorkItem workItem, IWorkItemService workItemService) : NotifyPropertyChanged
{
    public event EventHandler Deleted;
    public event EventHandler AddedToMyDay;
    public event EventHandler StartedAndAddedToMyDay;

    public Guid WorkItemId => workItem.Id;

    public string Name => workItem.Description;

    public ICommand AddToMyDay => new RelayCommand(() => AddedToMyDay?.Invoke(this, EventArgs.Empty));

    public ICommand StartAddToMyDay => new RelayCommand(() => StartedAndAddedToMyDay?.Invoke(this, EventArgs.Empty));

    public ICommand Delete => new RelayCommand(async () => 
    {
        await workItemService.Delete(WorkItemId);
        Deleted?.Invoke(this, EventArgs.Empty);
    });
}
