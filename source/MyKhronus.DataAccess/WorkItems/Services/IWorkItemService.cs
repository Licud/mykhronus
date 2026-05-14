namespace MyKhronus.DataAccess.WorkItems.Services;

using System.Threading.Tasks;

using MyKhronus.DataAccess.WorkItems.Models;

public interface IWorkItemService
{
    /// <summary>
    /// Adds a work item.
    /// </summary>
    /// <param name="workItem">Work item to add.</param>
    /// <returns>The </returns>
    Task<WorkItem> Add(NewWorkItem workItem);


    /// <summary>
    /// Gets all the work items that fits the given filter.
    /// </summary>
    /// <param name="filter">Filter to apply when retrieving work items.</param>
    /// <returns>A collection of work items that match the filter.</returns>
    Task<IEnumerable<WorkItem>> Get(WorkItemGetFilter filter);

    /// <summary>
    /// Searches for work items whose description contains the given text.
    /// </summary>
    /// <param name="description">Substring to search for in work item descriptions.</param>
    /// <returns>A collection of matching work items.</returns>
    Task<IEnumerable<WorkItem>> Search(string description);


    /// <summary>
    /// Deletes a work item.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to delete.</param>
    Task Delete(Guid workItemId);

    /// <summary>
    /// Link a work item to a project.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to link.</param>
    /// <param name="projectId">The ID of the project to link the work item to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LinkWorkItemToProject(Guid workItemId, int projectId);

    /// <summary>
    /// Unlink a work item to a project.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to link.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnlinkWorkItemToProject(Guid workItemId);
}
