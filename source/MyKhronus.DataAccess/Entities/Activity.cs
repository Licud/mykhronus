namespace MyKhronus.DataAccess.Entities;

using System.ComponentModel.DataAnnotations;

internal class Activity
{
    public Guid ActivityId { get; set; }

    [Required]
    public string Name { get; set; }

    public virtual ICollection<ActivityRecord> Records { get; set; } = new List<ActivityRecord>();
}
