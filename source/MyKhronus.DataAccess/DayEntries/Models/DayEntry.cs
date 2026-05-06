namespace MyKhronus.DataAccess.DayEntries.Models;

using System;

public record DayEntry(DateTime EntryDate, Guid WorkItemId, TimeSpan Duration);