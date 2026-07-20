using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>Persistence for application users (sign-in / sign-up).</summary>
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken = default);

    Task<int> InsertAsync(User user, CancellationToken cancellationToken = default);

    Task UpdateLastLoginAsync(int userId, DateTime whenUtc, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
