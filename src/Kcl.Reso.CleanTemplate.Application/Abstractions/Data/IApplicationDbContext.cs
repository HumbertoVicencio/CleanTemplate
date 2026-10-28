using Kcl.Reso.CleanTemplate.Domain.Todos;
using Kcl.Reso.CleanTemplate.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Kcl.Reso.CleanTemplate.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
