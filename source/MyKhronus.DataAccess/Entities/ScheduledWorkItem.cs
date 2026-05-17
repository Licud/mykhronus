namespace MyKhronus.DataAccess.Entities;

using System;

internal class ScheduledWorkItem
{
    public Guid WorkItemId { get; set; }

    public int Order { get; set; }

    public virtual WorkItem WorkItem { get; set; }
}
