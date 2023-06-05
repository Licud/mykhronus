namespace MyKhronus.WPF.Utilities;

using Microsoft.Extensions.DependencyInjection;

using MyKhronus.WPF.UserControls.ViewModels;

public class ViewModelLocator
{
    public MainWindowViewModel MainViewModel
        => App.ServiceProvider.GetRequiredService<MainWindowViewModel>();

    public ActivityUserControlViewModel ActivityUserControlViewModel
        => App.ServiceProvider.GetRequiredService<ActivityUserControlViewModel>();

    public ReportsUserControlViewModel ReportsUserControlViewModel
        => App.ServiceProvider.GetRequiredService<ReportsUserControlViewModel>();
}
