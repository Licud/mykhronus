namespace MyKhronus.DataAccess.Extensions;

using Microsoft.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    internal static void RemovePluralizingNameConventions(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.DisplayName());
        }
    }
}
