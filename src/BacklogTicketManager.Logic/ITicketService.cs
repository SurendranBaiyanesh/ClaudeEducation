using BacklogTicketManager.Common;

namespace BacklogTicketManager.Logic;

/// <summary>
/// Application-level ticket operations consumed by the UI (controls/pages). Wraps the
/// repository layer with business rules (e.g. stamping CreatedDate, flipping status to
/// Delayed) so pages never talk to Data.Repositories directly.
/// </summary>
public interface ITicketService
{
    Task<List<Ticket>> GetAllTicketsAsync();

    Task<Ticket?> GetTicketAsync(int id);

    Task<List<TicketUpdate>> GetUpdatesAsync(int ticketId);

    Task<int> CreateTicketAsync(Ticket ticket);

    Task UpdateTicketAsync(Ticket ticket);

    Task AddUpdateAsync(int ticketId, string updateText, string updatedBy);

    Task ChangeStatusAsync(int ticketId, TicketStatus newStatus);
}
