namespace MyKhronus.DataAccess.WorkItems.Models;

using System;

public record WorkItemGetFilter
{
    public Guid? WorkItemId { get; set; }

    public string Description { get; set; } = string.Empty;
}
