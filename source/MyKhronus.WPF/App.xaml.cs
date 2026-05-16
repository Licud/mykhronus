namespace MyKhronus.WPF;

using System.IO;
using System.Windows;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyKhronus.DataAccess.DependencyInjection;
using MyKhronus.WPF.Builders;
using MyKhronus.WPF.UserControls.ViewModels;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public IConfiguration Configuration { get; private set; }

    public void Application_Startup(object sender, StartupEventArgs e) { }

    protected override void OnStartup(StartupEventArgs e)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());

        Configuration = builder.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();

        ServiceProvider.EnsureMyKhronusDatabaseCreated();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");

        Directory.CreateDirectory(dataDirectory);

        var databaseLocation = Path.Combine(dataDirectory, "MyKhronus.db");

        var connectionString = $"Data Source={databaseLocation}";

        services.UsingMyKhronusDataAccess(configure =>
        {
            configure.ConnectionString = connectionString;
        });

        services.AddTransient<ProjectPickerViewModelFactory>();
        services.AddTransient<DayEntryViewModelFactory>();
        services.AddTransient<RecentWorkItemViewModelFactory>();

        services.AddScoped<RecentsViewModel>();
        services.AddScoped<ScheduledViewModel>();
        services.AddScoped<ReportsUserControlViewModel>();
        services.AddScoped<DayUserControlViewModel>();
        services.AddTransient<MainWindowViewModel>();

        services.AddTransient(typeof(MainWindow));
    }
}
