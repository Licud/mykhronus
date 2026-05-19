namespace MyKhronus.DataAccess.DayEntries.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.DataUtility;
using MyKhronus.DataAccess.DayEntries.Models;

internal class DailyEntryService(IUnitOfWorkFactory unitOfWorkFactory) : IDailyEntryService
{

    public async Task<DayEntry> Add(DateTime entryDate, Guid workItemId)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        var added = await unitOfWork.DailyEntries.Add(entryDate, workItemId);

        var workItems = await unitOfWork.WorkItems.Get(new()
        {
            WorkItemId = workItemId,
        });

        var updatedWorkItem = workItems.Single() with
        {
            LastUsed = DateTime.UtcNow,
        };

        await unitOfWork.WorkItems.Update(updatedWorkItem);

        await unitOfWork.Commit();

        return new DayEntry(entryDate, workItemId, added.Duration);
    }

    public async Task Delete(DateTime entryDate, Guid workItemId)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        await unitOfWork.DailyEntries.Delete(entryDate, workItemId);

        await unitOfWork.Commit();
    }

    public async Task<IReadOnlyList<DayEntry>> GetEntries(DateTime entryDate)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        return await unitOfWork.DailyEntries.GetEntries(entryDate);
    }

    public async Task<IReadOnlyList<DayEntry>> GetEntriesBetween(DateTime from, DateTime to)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        return await unitOfWork.DailyEntries.GetEntriesBetween(from, to);
    }

    public async Task Update(DayEntry dayEntry)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        await unitOfWork.DailyEntries.Update(dayEntry);

        await unitOfWork.Commit();
    }
}
