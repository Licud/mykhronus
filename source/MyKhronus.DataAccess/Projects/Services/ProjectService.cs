namespace MyKhronus.DataAccess.Projects.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.DataUtility;
using MyKhronus.DataAccess.Projects.Models;

internal class ProjectService(IUnitOfWork unitOfWork) : IProjectService
{
    public async Task<Project> Add(NewProject project)
    {
        var repository = unitOfWork.GetProjectRepository();

        var added = await repository.Add(project);

        await unitOfWork.Commit();

        return new Project(added.Id, added.Name);
    }

    public async Task Delete(int projectId)
    {
        var repository = unitOfWork.GetProjectRepository();

        await repository.Delete(projectId);

        await unitOfWork.Commit();
    }

    public async Task<IEnumerable<Project>> Get(ProjectGetFilter filter)
    {
        var repository = unitOfWork.GetProjectRepository();

        return await repository.Get(filter);
    }

    public Task<IEnumerable<Project>> Get()
    {
        return Get(new ProjectGetFilter());
    }
}
