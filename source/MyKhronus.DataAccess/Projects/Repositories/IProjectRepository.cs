namespace MyKhronus.DataAccess.Projects.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.Projects.Models;

internal interface IProjectRepository
{
    Task<Entities.Project> Add(NewProject project);

    Task<IEnumerable<Project>> Get(ProjectGetFilter filter);

    Task Delete(int projectId);
}
