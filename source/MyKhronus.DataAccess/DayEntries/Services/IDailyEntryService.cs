namespace MyKhronus.DataAccess.DayEntries.Services;

using MyKhronus.DataAccess.DayEntries.Models;

public interface IDailyEntryService
{
    Task<IReadOnlyList<DayEntry>> GetEntries(DateTime entryDate);

    Task<IReadOnlyList<DayEntry>> GetEntriesBetween(DateTime from, DateTime to);

    Task Delete(DateTime entryDate, Guid workItemId);

    Task<DayEntry> Add(DateTime entryDate, Guid workItemId);

    Task Update(DayEntry dayEntry);
}
