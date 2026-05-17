namespace MyKhronus.DataAccess.ScheduledWorkItems.Models;

using System;

using MyKhronus.DataAccess.WorkItems.Models;

public record ScheduledWorkItem(WorkItem WorkItem, int Order);