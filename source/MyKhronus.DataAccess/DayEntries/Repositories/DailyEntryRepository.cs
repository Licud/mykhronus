namespace MyKhronus.DataAccess.DayEntries.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.DayEntries.Models;

internal class DailyEntryRepository(
    ILogger<DailyEntryRepository> logger,
    MyKhronusContext context) : IDailyEntryRepository
{
    public async Task<DayEntry> Add(DateTime entryDate, Guid workItemId)
    {
        logger.LogTrace("Adding entry for date {entryDate} and work item {workItemId}", entryDate, workItemId);

        var saved = await context.DayEntries.FindAsync(entryDate, workItemId);

        if (saved == null)
        {
            saved = context.DayEntries.Add(new()
            {
                EntryDate = entryDate,
                WorkItemId = workItemId,
                Duration = TimeSpan.Zero,
            }).Entity;
        }

        var result = new DayEntry(entryDate, workItemId, saved.Duration);

        logger.LogInformation("Added entry for date {entryDate} and work item {workItemId}", entryDate, workItemId);

        return result;
    }

    public async Task Delete(DateTime entryDate, Guid workItemId)
    {
        logger.LogTrace("Deleting entry for date {entryDate} and work item {workItemId}", entryDate, workItemId);

        var entry = await context.DayEntries.FindAsync(entryDate, workItemId);

        if (entry != null)
        {
            context.DayEntries.Remove(entry);
        }

        logger.LogInformation("Deleted entry for date {entryDate} and work item {workItemId}", entryDate, workItemId);
    }

    public async Task Delete(Guid workItemId)
    {
        logger.LogTrace("Deleting entries for work item {workItemId}", workItemId);

        var entries = await context.DayEntries
            .Where(e => e.WorkItemId == workItemId)
            .ToListAsync();

        foreach (var entry in entries)
        {
            context.DayEntries.Remove(entry);
        }

        logger.LogInformation("Deleted entries for work item {workItemId}", workItemId);
    }

    public async Task<IReadOnlyList<DayEntry>> GetEntries(DateTime entryDate)
    {
        logger.LogTrace("Getting entries for date {entryDate}", entryDate);

        var query = context.DayEntries.AsNoTracking().Where(e => e.EntryDate == entryDate);

        var result = await query
            .Select(e => new DayEntry(e.EntryDate, e.WorkItemId, e.Duration))
            .ToListAsync();

        logger.LogDebug("Got {count} entries for date {entryDate}", result.Count, entryDate);

        return (IReadOnlyList<DayEntry>)result;
    }

    public async Task<IReadOnlyList<DayEntry>> GetEntriesBetween(DateTime from, DateTime to)
    {
        logger.LogTrace("Getting entries between {from} and {to}", from, to);

        var fromDate = from.Date;
        var toDate = to.Date;

        var query = context.DayEntries.AsNoTracking().Where(e => e.EntryDate >= fromDate && e.EntryDate <= toDate);

        var result = await query
            .Select(e => new DayEntry(e.EntryDate, e.WorkItemId, e.Duration))
            .ToListAsync();

        logger.LogDebug("Got {count} entries between {from} and {to}", result.Count, from, to);

        return (IReadOnlyList<DayEntry>)result;
    }

    public async Task Update(DayEntry dayEntry)
    {
        logger.LogTrace(
            "Updating entry for date {entryDate} and work item {workItemId}", 
            dayEntry.EntryDate, 
            dayEntry.WorkItemId);

        var saved = await context.DayEntries.FindAsync(dayEntry.EntryDate, dayEntry.WorkItemId);

        if (saved == null)
        {
            logger.LogWarning(
                "Entry for date {entryDate} and work item {workItemId} not found", 
                dayEntry.EntryDate, 
                dayEntry.WorkItemId);

            return;
        }

        saved.Duration = dayEntry.Duration;

        context.Update(saved);

        logger.LogInformation(
            "Updated entry for date {entryDate} and work item {workItemId}", 
            dayEntry.EntryDate, 
            dayEntry.WorkItemId);
    }
}
