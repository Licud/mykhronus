namespace MyKhronus.DataAccess.WorkItems.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.WorkItems.Models;

internal class WorkItemRepository(ILogger<WorkItemRepository> logger, MyKhronusContext context) : IWorkItemRepository
{
    public Task<WorkItem> Add(NewWorkItem workItem)
    {
        logger.LogTrace("Adding work item with description: {Description}", workItem.Description);

        var added = context.WorkItems.Add(new Entities.WorkItem()
        {
            Id = Guid.NewGuid(),
            Description = workItem.Description,
            LastUsed = DateTime.UtcNow,
        });

        var model = new WorkItem(added.Entity.Id, workItem.Description, added.Entity.LastUsed);
        
        logger.LogInformation("Work item added with ID: {Id}", model.Id);

        return Task.FromResult(model);
    }

    public async Task Delete(Guid workItemId)
    {
        logger.LogTrace("Deleting work item with ID: {Id}", workItemId);

        var workItem = await context.WorkItems.FindAsync(workItemId);

        if (workItem != null)
        {
            context.WorkItems.Remove(workItem);
        }

        logger.LogInformation("Work item with ID: {Id} deleted", workItemId);
    }

    public async Task<IEnumerable<WorkItem>> Get(WorkItemGetFilter filter)
    {
        logger.LogTrace("Getting work items with filter: {@Filter}", filter);

        var query = context.WorkItems.AsQueryable();

        if (filter.WorkItemId.HasValue)
        {
            query = query.Where(w => w.Id == filter.WorkItemId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Description))
        {
            query = query.Where(w => w.Description == filter.Description);
        }

        var models = await query
            .Select(w => new WorkItem(w.Id, w.Description, w.LastUsed))
            .ToListAsync();

        logger.LogDebug("Retrieved {Count} work items with filter: {@Filter}", models.Count, filter);

        return models;
    }

    public async Task LinkWorkItemToProject(Guid workItemId, int projectId)
    {
        logger.LogTrace("Linking work item with ID: {WorkItemId} to project with ID: {ProjectId}", workItemId, projectId);

        var item = await context.WorkItems.FindAsync(workItemId);

        item.ProjectId = projectId;

        context.Update(item);

        logger.LogInformation("Work item with ID: {WorkItemId} linked to project with ID: {ProjectId}", workItemId, projectId);
    }
}
