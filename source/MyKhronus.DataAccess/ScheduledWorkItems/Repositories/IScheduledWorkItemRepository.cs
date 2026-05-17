namespace MyKhronus.DataAccess.ScheduledWorkItems.Repositories;

using MyKhronus.DataAccess.ScheduledWorkItems.Models;

internal interface IScheduledWorkItemRepository
{
    Task<Entities.ScheduledWorkItem> Add(Guid workItemId);

    Task Delete(Guid workItemId);

    Task Reorder(IEnumerable<ScheduledWorkItem> scheduledWorkItems);

    Task<IEnumerable<ScheduledWorkItem>> Get();
}
