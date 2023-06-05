namespace MyKhronus.WPF;

using System.Windows.Input;

using MyKhronus.Commons.Utilities;
using MyKhronus.WPF.UserControls.ViewModels;
using MyKhronus.WPF.Utilities;

public class MainWindowViewModel : NotifyPropertyChanged
{
    private readonly ActivityUserControlViewModel activityViewModel;
    private readonly ReportsUserControlViewModel reportsViewModel;

    public MainWindowViewModel(ActivityUserControlViewModel activityViewModel,
        ReportsUserControlViewModel reportsViewModel)
    {
        this.activityViewModel = activityViewModel;
        this.reportsViewModel = reportsViewModel;

        SelectedViewModel = this.activityViewModel;
    }

    private MainViewModelControls selectedViewModel;

    public MainViewModelControls SelectedViewModel
    {
        get { return selectedViewModel; }
        set
        {
            selectedViewModel = value;
            OnPropertyChanged();
        }
    }

    public ICommand ShowActivityView
        => new RelayCommand(() => SelectedViewModel = activityViewModel);

    public ICommand ShowReportsView
        => new RelayCommand(() => SelectedViewModel = reportsViewModel);

}
