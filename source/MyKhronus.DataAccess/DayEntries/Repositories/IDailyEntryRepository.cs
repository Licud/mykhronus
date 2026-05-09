namespace MyKhronus.DataAccess.DayEntries.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.DayEntries.Models;

internal interface IDailyEntryRepository
{
    Task<IReadOnlyList<DayEntry>> GetEntries(DateTime entryDate);

    Task<IReadOnlyList<DayEntry>> GetEntriesBetween(DateTime from, DateTime to);

    Task Delete(DateTime entryDate, Guid workItemId);

    Task Delete(Guid workItemId);

    Task<DayEntry> Add(DateTime entryDate, Guid workItemId);

    Task Update(DayEntry dayEntry);
}
