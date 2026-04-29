namespace MyKhronus.DataAccess.Entities;

using System.Collections.Generic;

internal class Project
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<WorkItem> WorkItems { get; set; }
}
