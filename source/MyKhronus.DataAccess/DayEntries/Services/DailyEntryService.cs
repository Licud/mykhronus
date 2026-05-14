namespace MyKhronus.DataAccess.DayEntries.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.DataUtility;
using MyKhronus.DataAccess.DayEntries.Models;

internal class DailyEntryService(IUnitOfWork unitOfWork) : IDailyEntryService
{
    public async Task<DayEntry> Add(DateTime entryDate, Guid workItemId)
    {
        var dailyEntryRepository = unitOfWork.GetDailyEntryRepository();

        var added = await dailyEntryRepository.Add(entryDate, workItemId);

        var workItemRepository = unitOfWork.GetWorkItemRepository();

        var workItems = await workItemRepository.Get(new()
        {
            WorkItemId = workItemId,
        });

        var updatedWorkItem = workItems.Single() with
        {
            LastUsed = DateTime.UtcNow,
        };

        await workItemRepository.Update(updatedWorkItem);

        await unitOfWork.Commit();

        return  new DayEntry(entryDate, workItemId, added.Duration); ;
    }

    public async Task Delete(DateTime entryDate, Guid workItemId)
    {
        var repository = unitOfWork.GetDailyEntryRepository();

        await repository.Delete(entryDate, workItemId);

        await unitOfWork.Commit();
    }

    public async Task<IReadOnlyList<DayEntry>> GetEntries(DateTime entryDate)
    {
        var repository = unitOfWork.GetDailyEntryRepository();

        return await repository.GetEntries(entryDate);
    }

    public async Task<IReadOnlyList<DayEntry>> GetEntriesBetween(DateTime from, DateTime to)
    {
        var repository = unitOfWork.GetDailyEntryRepository();

        return await repository.GetEntriesBetween(from, to);
    }

    public async Task Update(DayEntry dayEntry)
    {
        var repository = unitOfWork.GetDailyEntryRepository();

        await repository.Update(dayEntry);

        await unitOfWork.Commit();
    }
}
