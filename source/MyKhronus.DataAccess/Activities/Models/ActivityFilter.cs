namespace MyKhronus.DataAccess.Activities.Models;

public record ActivityFilter
{
    public Guid? Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateTime? WithRecordsOnDate { get; init; }
}
