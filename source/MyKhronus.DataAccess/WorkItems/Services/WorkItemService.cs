namespace MyKhronus.DataAccess.WorkItems.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.DataUtility;
using MyKhronus.DataAccess.WorkItems.Models;

internal class WorkItemService(IUnitOfWork unitOfWork) : IWorkItemService
{
    public async Task<WorkItem> Add(NewWorkItem workItem)
    {
        var repository = unitOfWork.GetWorkItemRepository();

        var filter = new WorkItemGetFilter
        {
            Description = workItem.Description,
        };

        var existing = await repository.Get(filter);

        if (existing.Any())
        {
            return existing.FirstOrDefault();
        }

        var added = await repository.Add(workItem);

        await unitOfWork.Commit();

        return new WorkItem(added.Id, added.Description, added.LastUsed);
    }

    public async Task Delete(Guid workItemId)
    {
        var dailyEntryRepository = unitOfWork.GetDailyEntryRepository();

        await dailyEntryRepository.Delete(workItemId);

        var workItemRepository = unitOfWork.GetWorkItemRepository();

        await workItemRepository.Delete(workItemId);

        await unitOfWork.Commit();
    }

    public async Task<IEnumerable<WorkItem>> Get(WorkItemGetFilter filter)
    {
        var repository = unitOfWork.GetWorkItemRepository();

        return await repository.Get(filter);
    }

    public async Task<IEnumerable<WorkItem>> Search(string description)
    {
        var repository = unitOfWork.GetWorkItemRepository();

        return await repository.Search(description);
    }

    public async Task LinkWorkItemToProject(Guid workItemId, int projectId)
    {
        var repository = unitOfWork.GetWorkItemRepository();

        await repository.LinkWorkItemToProject(workItemId, projectId);

        await unitOfWork.Commit();
    }

    public async Task UnlinkWorkItemToProject(Guid workItemId)
    {
        var repository = unitOfWork.GetWorkItemRepository();

        await repository.UnlinkWorkItemToProject(workItemId);

        await unitOfWork.Commit();
    }
}
