namespace MyKhronus.DataAccess.Entities;

using System.ComponentModel.DataAnnotations;

internal class Activity
{
    public Guid ActivityId { get; set; }

    [Required]
    public string Name { get; set; }
}
