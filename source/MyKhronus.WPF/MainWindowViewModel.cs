namespace MyKhronus.WPF;

using System.Windows.Input;

using MyKhronus.WPF.UserControls.ViewModels;
using MyKhronus.WPF.Utilities;

public class MainWindowViewModel(
    DayUserControlViewModel dayViewModel,
    ReportsUserControlViewModel reportsViewModel) 
    : NotifyPropertyChanged
{
    private MainViewModelControls selectedViewModel = dayViewModel;

    public MainViewModelControls SelectedViewModel
    {
        get { return selectedViewModel; }
        set
        {
            selectedViewModel = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsDayViewActive));
            OnPropertyChanged(nameof(IsReportsViewActive));
        }
    }

    public bool IsDayViewActive => selectedViewModel == dayViewModel;

    public bool IsReportsViewActive => selectedViewModel == reportsViewModel;

    public ICommand ShowReportsView
        => new RelayCommand(() => SelectedViewModel = reportsViewModel);

    public ICommand ShowDayView
        => new RelayCommand(() => SelectedViewModel = dayViewModel);

}
