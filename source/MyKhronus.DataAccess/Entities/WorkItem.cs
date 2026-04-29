namespace MyKhronus.DataAccess.Entities;

using System;

internal class WorkItem
{
    public Guid Id { get; set; }

    public string Description { get; set; }

    public DateTime LastUsed { get; set; }
}
