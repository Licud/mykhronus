namespace MyKhronus.DataAccess.WorkItems.Models;

using System;

public record WorkItemGetFilter
{
    public Guid? WorkItemId { get; set; }

    public string Name { get; set; } = string.Empty;
}
