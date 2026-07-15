using BacklogTicketManager.Data.Repositories;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Logic;

/// <summary>
/// Default <see cref="ITicketService"/> implementation. Depends only on repository and
/// notification abstractions - never on ADO.NET or SMTP directly (DIP), and has a single
/// reason to change: ticket business rules (SRP).
/// </summary>
public sealed class TicketService : ITicketService
{
    private readonly ITicketRepository _tickets;
    private readonly ITicketUpdateRepository _updates;
    private readonly ITicketNotificationService _notifications;

    public TicketService(
        ITicketRepository tickets,
        ITicketUpdateRepository updates,
        ITicketNotificationService notifications)
    {
        _tickets = tickets;
        _updates = updates;
        _notifications = notifications;
    }

    public Task<List<Ticket>> GetAllTicketsAsync() => _tickets.GetAllAsync();

    public Task<Ticket?> GetTicketAsync(int id) => _tickets.GetByIdAsync(id);

    public Task<List<TicketUpdate>> GetUpdatesAsync(int ticketId) => _updates.GetByTicketIdAsync(ticketId);

    public async Task<int> CreateTicketAsync(Ticket ticket)
    {
        ticket.CreatedDate = DateTime.UtcNow;
        ticket.Status = TicketStatus.New;
        var id = await _tickets.InsertAsync(ticket);
        return id;
    }

    public Task UpdateTicketAsync(Ticket ticket) => _tickets.UpdateAsync(ticket);

    public async Task AddUpdateAsync(int ticketId, string updateText, string updatedBy)
    {
        var update = new TicketUpdate
        {
            TicketId = ticketId,
            UpdateText = updateText,
            UpdatedBy = updatedBy,
            UpdatedDate = DateTime.UtcNow
        };

        await _updates.InsertAsync(update);
    }

    public async Task ChangeStatusAsync(int ticketId, TicketStatus newStatus)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId)
            ?? throw new InvalidOperationException($"Ticket {ticketId} was not found.");

        var previousStatus = ticket.Status;
        ticket.Status = newStatus;

        if (newStatus == TicketStatus.Completed)
        {
            ticket.CompletedDate = DateTime.UtcNow;
        }

        await _tickets.UpdateAsync(ticket);

        if (previousStatus != newStatus)
        {
            await _notifications.NotifyStatusChangedAsync(ticket, previousStatus);
        }
    }
}
