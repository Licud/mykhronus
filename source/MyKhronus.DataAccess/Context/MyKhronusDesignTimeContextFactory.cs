namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class MyKhronusDesignTimeContextFactory : IDesignTimeDbContextFactory<MyKhronusContext_old>
{
    public MyKhronusContext_old CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MyKhronusContext_old>();

        var databaseName = "MyKhronus";

        var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Dev\\MyKhronus\\Source\\MyKhronus\\MyKhronus.DataAccess\\MyKhronusData.mdf;Integrated Security=True";

        builder.UseSqlServer(connectionString);

        return new MyKhronusContext_old(builder.Options);
    }
}
