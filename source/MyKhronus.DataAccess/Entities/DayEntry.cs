namespace MyKhronus.DataAccess.Entities;

using System;

internal class DayEntry
{
    public DateTime EntryDate { get; set; }

    public Guid WorkItemId { get; init; }

    public TimeSpan Duration { get; set; }
}
