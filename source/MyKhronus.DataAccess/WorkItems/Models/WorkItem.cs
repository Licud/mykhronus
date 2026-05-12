namespace MyKhronus.DataAccess.WorkItems.Models;

using System;

using MyKhronus.DataAccess.Projects.Models;

public record WorkItem(Guid Id, string Description, DateTime LastUsed, Project Project = null);
