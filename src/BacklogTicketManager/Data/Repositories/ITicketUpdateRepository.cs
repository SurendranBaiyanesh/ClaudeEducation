using BacklogTicketManager.Models;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>Persistence operations for ticket timeline entries.</summary>
public interface ITicketUpdateRepository
{
    Task<List<TicketUpdate>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default);

    Task<int> InsertAsync(TicketUpdate update, CancellationToken cancellationToken = default);
}
