namespace MyKhronus.DataAccess.ActivityRecords.Models;

using MyKhronus.DataAccess.Activities.Models;

public record ActivityRecord
{
    public Guid ActivityRecordId { get; init; }

    public DateTime RecordDate { get; init; }

    public string Description { get; init; }

    public TimeSpan Duration { get; init; }

    public Activity Activity { get; init; }

}