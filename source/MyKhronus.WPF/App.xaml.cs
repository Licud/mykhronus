namespace MyKhronus.WPF;

using System.IO;
using System.Windows;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.DependencyInjection;
using MyKhronus.DataAccess.Services;
using MyKhronus.WPF.Services;
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
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var databaseLocation = Path.Combine(Environment.CurrentDirectory, "Data", "MyKhronusData.mdf");

        if (!File.Exists(databaseLocation))
        {
            throw new ApplicationException($"The database path {databaseLocation} could not be found.");
        }

        var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={databaseLocation};Integrated Security=True";

        services.AddDbContextFactory<MyKhronusContext_old>(options => options.UseSqlServer(connectionString));

        services.UsingMyKhronusDataAccess(configure =>
        {
            configure.ConnectionString = connectionString;
        });

        services.AddScoped<IActivityRecordTimerService, ActivityRecordTimerService>();
        services.AddScoped<IActivityService, ActivityServices>();
        services.AddScoped<IActivityRecordsService, ActivityRecordsService>();
        services.AddScoped<ILockedSessionTimerService, LockedSessionTimerService>();
        services.AddScoped<IAutoSaveService, AutoSaveService>();

        services.AddScoped<ActivityUserControlViewModel>();
        services.AddScoped<ReportsUserControlViewModel>();
        services.AddTransient<MainWindowViewModel>();

        services.AddTransient(typeof(MainWindow));
    }
}
