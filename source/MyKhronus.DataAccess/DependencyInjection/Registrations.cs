namespace MyKhronus.DataAccess.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using MyKhronus.DataAccess.ActivityRecords.Repositories;
using MyKhronus.DataAccess.ActivityRecords.Services;
using MyKhronus.DataAccess.DataUtility;

public static class Registrations
{
    public static IServiceCollection UsingMyKhronusDataAccess(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IActivityRecordService, ActivityRecordService>();
        serviceCollection.AddTransient<IActivityRecordRepository, ActivityRecordRepository>();

        serviceCollection.AddTransient<IUnitOfWork, MyKhronusContextUnitOfWork>();

        return serviceCollection;
    }
}
