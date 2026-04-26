namespace MyKhronus.DataAccess.Entities;

using System.ComponentModel.DataAnnotations;

internal class ActivityRecord
{
    public Guid ActivityRecordId { get; set; }

    [Required]
    public DateTime RecordDate { get; set; }

    public string Description { get; set; }

    public TimeSpan Duration { get; set; }

    [Required]
    public Guid ActivityId { get; set; }

    public virtual Activity Activity { get; set; }
}
