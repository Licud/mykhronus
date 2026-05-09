namespace MyKhronus.WPF.Utilities;

using Microsoft.Extensions.DependencyInjection;

using MyKhronus.WPF.UserControls.ViewModels;

public class ViewModelLocator
{
    public MainWindowViewModel MainViewModel
        => App.ServiceProvider.GetRequiredService<MainWindowViewModel>();

    public ReportsUserControlViewModel ReportsUserControlViewModel
        => App.ServiceProvider.GetRequiredService<ReportsUserControlViewModel>();

    public DayUserControlViewModel DayUserControlViewModel
        => App.ServiceProvider.GetRequiredService<DayUserControlViewModel>();
}
