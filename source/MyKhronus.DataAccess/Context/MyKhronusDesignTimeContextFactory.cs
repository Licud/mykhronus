namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class MyKhronusDesignTimeContextFactory : IDesignTimeDbContextFactory<MyKhronusContext>
{
    public MyKhronusContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MyKhronusContext>();

        var databaseName = "MyKhronus";

        var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Dev\\MyKhronus\\Source\\MyKhronus\\MyKhronus.DataAccess\\MyKhronusData.mdf;Integrated Security=True";

        builder.UseSqlServer(connectionString);

        return new MyKhronusContext(builder.Options);
    }
}
