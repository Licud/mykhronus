namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Windows;
using System.Windows.Input;

using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Utilities;

public class DayUserControlViewModel(IWorkItemService workItemService) : MainViewModelControls
{
    private string newEntryName;

    public string NewEntryName
    {
        get => newEntryName;
        set
        {
            newEntryName = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddNewEntry => new RelayCommand(async () =>
    {
        try
        {
            await AddWorkItem();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    });

    public ICommand AddAndStartNewEntry => new RelayCommand(async () => 
    {
        try
        {
            await AddWorkItem();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    });

    private async Task<WorkItem> AddWorkItem()
    {
        var newWorkItem = new NewWorkItem(NewEntryName.Trim());

        var addedWorkItem = await workItemService.Add(newWorkItem);

        NewEntryName = string.Empty;

        return addedWorkItem;
    }
}
