namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

internal sealed class MyKhronusDesignTimeContextFactory : IDesignTimeDbContextFactory<MyKhronusContext>
{
    public MyKhronusContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MyKhronusContext>();

        var filePath = @"C:\Dev\mykhronus\source\MyKhronus.DataAccess\MyKhronusData2.mdf";
        var initialCatalog = "MyKhronusData2";

        var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={filePath};Initial Catalog={initialCatalog};Integrated Security=True";

        builder.UseSqlServer(connectionString);

        return new MyKhronusContext(builder.Options);
    }
}
