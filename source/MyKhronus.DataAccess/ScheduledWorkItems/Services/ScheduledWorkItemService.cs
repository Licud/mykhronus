namespace MyKhronus.DataAccess.ScheduledWorkItems.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyKhronus.DataAccess.DataUtility;
using MyKhronus.DataAccess.ScheduledWorkItems.Models;

internal class ScheduledWorkItemService(IUnitOfWork unitOfWork) : IScheduledWorkItemService
{
    public async Task<IReadOnlyList<ScheduledWorkItem>> Get()
    {
        var repository = unitOfWork.GetScheduledWorkItemRepository();

        var scheduledWorkItems = await repository.Get();

        return scheduledWorkItems
            .OrderBy(s => s.Order)
            .ToList();
    }

    public async Task<ScheduledWorkItem> Add(Guid workItemId)
    {
        var repository = unitOfWork.GetScheduledWorkItemRepository();

        var added = await repository.Add(workItemId);

        await unitOfWork.Commit();

        var workItemRepository = unitOfWork.GetWorkItemRepository();

        var workItems = await workItemRepository.Get(new()
        {
            WorkItemId = workItemId,
        });

        return new ScheduledWorkItem(workItems.Single(), added.Order);
    }

    public async Task Delete(Guid workItemId)
    {
        var repository = unitOfWork.GetScheduledWorkItemRepository();

        await repository.Delete(workItemId);

        await unitOfWork.Commit();
    }

    public async Task Reorder(IEnumerable<ScheduledWorkItem> scheduledWorkItems)
    {
        var repository = unitOfWork.GetScheduledWorkItemRepository();

        await repository.Reorder(scheduledWorkItems);

        await unitOfWork.Commit();
    }
}
