namespace MyKhronus.DataAccess.WorkItems.Models;

using System;

using MyKhronus.DataAccess.Shared;

public record WorkItemGetFilter
{
    public Option<Guid> WorkItemId { get; init; }

    public Option<string> Description { get; init; }

    public Option<IEnumerable<Guid>> WorkItemIds { get; init; }
}
