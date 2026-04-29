namespace MyKhronus.DataAccess.WorkItems.Models;

using System;

public record WorkItem(Guid Id, string Description, DateTime LastUsed);
