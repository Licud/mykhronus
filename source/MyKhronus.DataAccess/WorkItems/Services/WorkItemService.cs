namespace MyKhronus.DataAccess.WorkItems.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.DataUtility;
using MyKhronus.DataAccess.WorkItems.Models;

internal class WorkItemService(IUnitOfWorkFactory unitOfWorkFactory) : IWorkItemService
{
    public async Task<WorkItem> Add(NewWorkItem workItem)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        var filter = new WorkItemGetFilter
        {
            Description = workItem.Description,
        };

        var existing = await unitOfWork.WorkItems.Get(filter);

        if (existing.Any())
        {
            return existing.FirstOrDefault();
        }

        var added = await unitOfWork.WorkItems.Add(workItem);

        await unitOfWork.Commit();

        return new WorkItem(added.Id, added.Description, added.LastUsed);
    }

    public async Task Delete(Guid workItemId)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        await unitOfWork.DailyEntries.Delete(workItemId);

        await unitOfWork.WorkItems.Delete(workItemId);

        await unitOfWork.Commit();
    }

    public async Task<IEnumerable<WorkItem>> Get(WorkItemGetFilter filter)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        return await unitOfWork.WorkItems.Get(filter);
    }

    public async Task<IEnumerable<WorkItem>> Search(string description)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        return await unitOfWork.WorkItems.Search(description);
    }

    public async Task LinkWorkItemToProject(Guid workItemId, int projectId)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        await unitOfWork.WorkItems.LinkWorkItemToProject(workItemId, projectId);

        await unitOfWork.Commit();
    }

    public async Task UnlinkWorkItemToProject(Guid workItemId)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        await unitOfWork.WorkItems.UnlinkWorkItemToProject(workItemId);

        await unitOfWork.Commit();
    }
}
