namespace MyKhronus.DataAccess.WorkItems.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.Projects.Models;
using MyKhronus.DataAccess.WorkItems.Models;

internal class WorkItemRepository(ILogger<WorkItemRepository> logger, MyKhronusContext context) : IWorkItemRepository
{
    public Task<Entities.WorkItem> Add(NewWorkItem workItem)
    {
        logger.LogTrace("Adding work item with description: {Description}", workItem.Description);

        var added = context.WorkItems.Add(new Entities.WorkItem()
        {
            Id = Guid.NewGuid(),
            Description = workItem.Description,
            LastUsed = DateTime.UtcNow,
        });

        logger.LogInformation("Work item with description: {Description} added", added.Entity.Description);

        return Task.FromResult(added.Entity);
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

    public async Task<IEnumerable<WorkItem>> Get(WorkItemGetFilter filter, int limit = 100)
    {
        logger.LogTrace("Getting work items with filter: {@Filter}", filter);

        var query = context.WorkItems
            .AsNoTracking()
            .Include(w => w.Project)
            .AsQueryable();

        if (filter.WorkItemId.HasValue)
        {
            query = query.Where(w => w.Id == filter.WorkItemId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Description))
        {
            query = query.Where(w => w.Description == filter.Description);
        }

        if (filter.WorkItemIds.Any())
        {
            query = query.Where(w => filter.WorkItemIds.Contains(w.Id));
        }

        query = query
            .OrderByDescending(q => q.LastUsed)
            .Take(limit);

        var results = await query.ToListAsync();

        var models = new List<WorkItem>();

        foreach (var item in results)
        {
            Project project = null;

            if (item.Project != null)
            {
                project = new Project(item.Project.Id, item.Project.Name);
            }

            models.Add(new WorkItem(item.Id, item.Description, item.LastUsed, project));
        }

        logger.LogDebug("Retrieved {Count} work items with filter: {@Filter}", models.Count, filter);

        return models;
    }

    public async Task<IEnumerable<WorkItem>> Search(string description, int limit = 50)
    {
        logger.LogTrace("Searching work items with description like: {Description}", description);

        var pattern = $"%{description}%";

        var results = await context.WorkItems
            .AsNoTracking()
            .Include(w => w.Project)
            .Where(w => EF.Functions.Like(w.Description, pattern))
            .OrderByDescending(q => q.LastUsed)
            .Take(limit)
            .ToListAsync();

        var models = new List<WorkItem>();

        foreach (var item in results)
        {
            Project project = null;

            if (item.Project != null)
            {
                project = new Project(item.Project.Id, item.Project.Name);
            }

            models.Add(new WorkItem(item.Id, item.Description, item.LastUsed, project));
        }

        logger.LogDebug("Search for description like: {Description} returned {Count} work items", description, models.Count);

        return models;
    }

    public async Task LinkWorkItemToProject(Guid workItemId, int projectId)
    {
        logger.LogTrace("Linking work item with ID: {WorkItemId} to project with ID: {ProjectId}", workItemId, projectId);

        var item = await context.WorkItems.FindAsync(workItemId);

        item.ProjectId = projectId;
        item.Project = await context.Projects.FindAsync(projectId);

        context.Update(item);

        logger.LogInformation("Work item with ID: {WorkItemId} linked to project with ID: {ProjectId}", workItemId, projectId);
    }

    public async Task UnlinkWorkItemToProject(Guid workItemId)
    {
        logger.LogTrace("Unlinking work item with ID: {WorkItemId} from its project", workItemId);

        var item = await context.WorkItems.FindAsync(workItemId);

        if (item != null)
        {
            item.ProjectId = null;
            item.Project = null;
            context.Update(item);
        }

        logger.LogInformation("Work item with ID: {WorkItemId} unlinked from its project", workItemId);
    }

    public async Task Update(WorkItem workItem)
    {
        logger.LogTrace("Updating work item with ID: {Id}", workItem.Id);

        var item = await context.WorkItems.FindAsync(workItem.Id);

        if (item != null)
        {
            item.LastUsed = workItem.LastUsed;
            item.Description = workItem.Description;
            context.Update(item);
        }

        logger.LogInformation("Work item with ID: {WorkItemId} updated", workItem.Id);
    }
}