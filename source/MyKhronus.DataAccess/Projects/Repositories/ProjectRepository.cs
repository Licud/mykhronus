namespace MyKhronus.DataAccess.Projects.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Context;
using MyKhronus.DataAccess.Projects.Models;

internal class ProjectRepository(ILogger<ProjectRepository> logger, MyKhronusContext context) : IProjectRepository
{
    public Task<Project> Add(NewProject project)
    {
        logger.LogTrace("Adding project with name: {Name}", project.Name);

        var added = context.Projects.Add(new Entities.Project()
        {
            Name = project.Name,
        });

        var model = new Project(added.Entity.Id, project.Name);

        logger.LogInformation("Project added with ID: {Id}", model.Id);

        return Task.FromResult(model);
    }

    public async Task Delete(int projectId)
    {
        logger.LogTrace("Deleting project with ID: {Id}", projectId);

        var project = await context.Projects.FindAsync(projectId);

        if (project != null)
        {
            context.Projects.Remove(project);
        }

        logger.LogInformation("Project with ID: {Id} deleted", projectId);
    }

    public async Task<IEnumerable<Project>> Get(ProjectGetFilter filter)
    {
        logger.LogTrace("Getting projects with filter: {@Filter}", filter);

        var query = context.Projects.AsQueryable();

        if (filter.Id.HasValue)
        {
            query = query.Where(p => p.Id == filter.Id.Value);
        }

        var models = await query
            .Select(p => new Project(p.Id, p.Name))
            .ToListAsync();

        logger.LogDebug("Retrieved {Count} projects with filter: {@Filter}", models.Count, filter);

        return models;
    }
}
