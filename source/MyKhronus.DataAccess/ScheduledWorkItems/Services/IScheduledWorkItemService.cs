namespace MyKhronus.DataAccess.ScheduledWorkItems.Services;

using MyKhronus.DataAccess.ScheduledWorkItems.Models;

public interface IScheduledWorkItemService
{
    Task<IReadOnlyList<ScheduledWorkItem>> Get();

    Task<ScheduledWorkItem> Add(Guid workItemId);

    Task Delete(Guid workItemId);

    Task Reorder(IEnumerable<ScheduledWorkItem> scheduledWorkItems);
}
