namespace MyKhronus.DataAccess.ActivityRecords.Models;

public record ActivityRecordFilter
{
    public Guid? ActivityId { get; init;}

    public DateTime? RecordDate { get; init; }

    public (DateTime From, DateTime To)? RecordDateRange { get; init; }
}
