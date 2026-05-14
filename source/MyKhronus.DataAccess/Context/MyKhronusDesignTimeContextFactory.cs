namespace MyKhronus.DataAccess.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

internal sealed class MyKhronusDesignTimeContextFactory : IDesignTimeDbContextFactory<MyKhronusContext>
{
    public MyKhronusContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MyKhronusContext>();

        var filePath = @"C:\Dev\mykhronus\source\MyKhronus.WPF\bin\Debug\net8.0-windows\Data\MyKhronus.db";

        var connectionString = $"Data Source={filePath}";

        builder.UseSqlite(connectionString);

        return new MyKhronusContext(builder.Options);
    }
}
