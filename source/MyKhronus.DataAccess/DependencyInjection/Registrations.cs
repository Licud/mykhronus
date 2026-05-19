namespace MyKhronus.DataAccess.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.DataUtility;
using MyKhronus.DataAccess.DayEntries.Services;
using MyKhronus.DataAccess.Projects.Services;
using MyKhronus.DataAccess.ScheduledWorkItems.Services;
using MyKhronus.DataAccess.WorkItems.Services;

public static class Registrations
{
    public static IServiceCollection UsingMyKhronusDataAccess(
        this IServiceCollection serviceCollection,
        Action<RegistrationBuilder> configure)
    {
        var builder = new RegistrationBuilder();
        configure(builder);

        serviceCollection.AddDbContextFactory<MyKhronusContext>(options => options.UseSqlite(builder.ConnectionString));

        serviceCollection.AddTransient<IWorkItemService, WorkItemService>();
        serviceCollection.AddTransient<IProjectService, ProjectService>();
        serviceCollection.AddTransient<IDailyEntryService, DailyEntryService>();
        serviceCollection.AddTransient<IScheduledWorkItemService, ScheduledWorkItemService>();

        serviceCollection.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();

        return serviceCollection;
    }

    public static void EnsureMyKhronusDatabaseCreated(this IServiceProvider provider)
    {
        var factory = provider.GetRequiredService<IDbContextFactory<MyKhronusContext>>();

        using var context = factory.CreateDbContext();

        context.Database.Migrate();
    }
}
