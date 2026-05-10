namespace MyKhronus.DataAccess.Projects.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using MyKhronus.DataAccess.Projects.Models;

public interface IProjectService
{
    Task<Project> Add(NewProject project);

    Task<IEnumerable<Project>> Get();

    Task<IEnumerable<Project>> Get(ProjectGetFilter filter);

    Task Delete(int projectId);
}
