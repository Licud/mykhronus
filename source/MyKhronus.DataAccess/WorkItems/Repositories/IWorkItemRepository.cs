namespace MyKhronus.DataAccess.WorkItems.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.WorkItems.Models;

internal interface IWorkItemRepository
{
    Task<Entities.WorkItem> Add(NewWorkItem workItem);

    Task<IEnumerable<WorkItem>> Get(WorkItemGetFilter filter);

    Task Delete(Guid workItemId);

    Task LinkWorkItemToProject(Guid workItemId, int projectId);

    Task UnlinkWorkItemToProject(Guid workItemId);
}
