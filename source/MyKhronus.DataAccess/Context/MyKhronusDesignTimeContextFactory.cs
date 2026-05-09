namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

internal sealed class MyKhronusDesignTimeContextFactory : IDesignTimeDbContextFactory<MyKhronusContext>
{
    public MyKhronusContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MyKhronusContext>();

        var filePath = @"C:\Dev\mykhronus\source\MyKhronus.WPF\bin\Debug\net8.0-windows\Data\MyKhronusData.mdf";

        var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={filePath};Initial Catalog=MyKhronusData;Integrated Security=True";

        builder.UseSqlServer(connectionString);

        return new MyKhronusContext(builder.Options);
    }
}
