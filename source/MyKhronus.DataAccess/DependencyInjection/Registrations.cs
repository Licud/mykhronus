namespace MyKhronus.DataAccess.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MyKhronus.DataAccess.ActivityRecords.Repositories;
using MyKhronus.DataAccess.ActivityRecords.Services;
using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.DataUtility;

public static class Registrations
{
    public static IServiceCollection UsingMyKhronusDataAccess(
        this IServiceCollection serviceCollection,
        Action<RegistrationBuilder> configure)
    {
        var builder = new RegistrationBuilder();
        configure(builder);

        serviceCollection.AddDbContextFactory<MyKhronusContext>(options => options.UseSqlServer(builder.ConnectionString));

        serviceCollection.AddTransient<IActivityRecordService, ActivityRecordService>();
        serviceCollection.AddTransient<IActivityRecordRepository, ActivityRecordRepository>();

        serviceCollection.AddTransient<IUnitOfWork, MyKhronusContextUnitOfWork>();

        return serviceCollection;
    }
}
