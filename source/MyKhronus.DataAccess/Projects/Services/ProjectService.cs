namespace MyKhronus.DataAccess.Projects.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.DataUtility;
using MyKhronus.DataAccess.Projects.Models;

internal class ProjectService(IUnitOfWorkFactory unitOfWorkFactory) : IProjectService
{
    public async Task<Project> Add(NewProject project)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        var added = await unitOfWork.Projects.Add(project);

        await unitOfWork.Commit();

        return new Project(added.Id, added.Name);
    }

    public async Task Delete(int projectId)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        await unitOfWork.Projects.Delete(projectId);

        await unitOfWork.Commit();
    }

    public async Task<IEnumerable<Project>> Get(ProjectGetFilter filter)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        return await unitOfWork.Projects.Get(filter);
    }

    public Task<IEnumerable<Project>> Get()
    {
        return Get(new ProjectGetFilter());
    }
}
