using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Application.Common.Interfaces;

public interface IAppDb
{
    DbSet<Project> Projects { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
