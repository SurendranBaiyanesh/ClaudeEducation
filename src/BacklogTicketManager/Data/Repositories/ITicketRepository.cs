using BacklogTicketManager.Models;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>
/// Persistence operations for <see cref="Ticket"/> records. Consumers depend on this
/// abstraction, never on ADO.NET types directly (Dependency Inversion Principle).
/// </summary>
public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Ticket?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Tickets that are overdue, or due within <paramref name="dueSoonWindow"/>, and not yet completed.</summary>
    Task<List<Ticket>> GetOverdueOrDueSoonAsync(TimeSpan dueSoonWindow, CancellationToken cancellationToken = default);

    Task<int> InsertAsync(Ticket ticket, CancellationToken cancellationToken = default);

    Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default);

    Task MarkNotifiedAsync(int ticketId, DateTime notifiedAtUtc, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
