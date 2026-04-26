namespace MyKhronus.DataAccess.ActivityRecords.Models;

public record NewActivityRecord
{
    public DateTime RecordDate { get; init; }

    public string Description { get; init; }

    public TimeSpan Duration { get; init; }

    public Guid ActivityId { get; init; }
}
