namespace MyKhronus.DataAccess.Activities.Models;

public record Activity
{
    public Guid ActivityId { get; init; }

    public string Name { get; init; }
}
