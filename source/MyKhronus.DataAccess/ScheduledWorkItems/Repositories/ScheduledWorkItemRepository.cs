namespace MyKhronus.DataAccess.ScheduledWorkItems.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.Projects.Models;
using MyKhronus.DataAccess.ScheduledWorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Models;

internal class ScheduledWorkItemRepository(
    ILogger<ScheduledWorkItemRepository> logger,
    MyKhronusContext context) 
    : IScheduledWorkItemRepository
{
    public async Task<Entities.ScheduledWorkItem> Add(Guid workItemId)
    {
        logger.LogTrace("Adding scheduled work item {workItemId}", workItemId);

        var saved = await context.ScheduledWorkItems.FindAsync(workItemId);

        if (saved == null)
        {
            var nextOrder = await context.ScheduledWorkItems.AnyAsync()
                ? await context.ScheduledWorkItems.MaxAsync(s => s.Order) + 1
                : 0;

            saved = context.ScheduledWorkItems.Add(new Entities.ScheduledWorkItem()
            {
                WorkItemId = workItemId,
                Order = nextOrder,
            }).Entity;
        }

        logger.LogInformation("Added scheduled work item {workItemId}", workItemId);

        return saved;
    }

    public async Task Delete(Guid workItemId)
    {
        logger.LogTrace("Deleting scheduled work item {workItemId}", workItemId);

        var scheduled = await context.ScheduledWorkItems.FindAsync(workItemId);

        if (scheduled != null)
        {
            context.ScheduledWorkItems.Remove(scheduled);
        }

        logger.LogInformation("Deleted scheduled work item {workItemId}", workItemId);
    }

    public async Task Reorder(IEnumerable<ScheduledWorkItem> scheduledWorkItems)
    {
        logger.LogTrace("Reordering scheduled work items");

        foreach (var scheduled in scheduledWorkItems)
        {
            var saved = await context.ScheduledWorkItems.FindAsync(scheduled.WorkItem.Id);

            if (saved == null)
            {
                logger.LogWarning("Scheduled work item {workItemId} not found", scheduled.WorkItem.Id);

                continue;
            }

            saved.Order = scheduled.Order;

            context.Update(saved);
        }

        logger.LogInformation("Reordered scheduled work items");
    }

    public async Task<IEnumerable<ScheduledWorkItem>> Get()
    {
        logger.LogTrace("Getting scheduled work items");

        var results = await context.ScheduledWorkItems
            .AsNoTracking()
            .Include(s => s.WorkItem)
                .ThenInclude(w => w.Project)
            .ToListAsync();

        var models = new List<ScheduledWorkItem>();

        foreach (var item in results)
        {
            Project project = null;

            if (item.WorkItem.Project != null)
            {
                project = new Project(item.WorkItem.Project.Id, item.WorkItem.Project.Name);
            }

            var workItem = new WorkItem(
                item.WorkItem.Id,
                item.WorkItem.Description,
                item.WorkItem.LastUsed,
                project);

            models.Add(new ScheduledWorkItem(workItem, item.Order));
        }

        logger.LogDebug("Retrieved {count} scheduled work items", models.Count);

        return models;
    }
}
