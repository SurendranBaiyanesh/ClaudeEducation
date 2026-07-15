using BacklogTicketManager.Common;

namespace BacklogTicketManager.Logic;

/// <summary>
/// Orchestrates automatic emails triggered by ticket events (delays, approaching
/// deadlines, status changes). This is the only service allowed to combine
/// <see cref="IEmailSender"/>, <see cref="IEmailTemplateBuilder"/> and the notification
/// audit repository.
/// </summary>
public interface ITicketNotificationService
{
    /// <summary>Scans all tickets and emails owners for anything overdue or due soon. Used by the background monitor.</summary>
    Task<int> ProcessDueAndOverdueTicketsAsync(CancellationToken cancellationToken = default);

    Task NotifyStatusChangedAsync(Ticket ticket, TicketStatus previousStatus, CancellationToken cancellationToken = default);

    /// <summary>Read-only audit trail for the Email Log view, so pages never depend on Data.Repositories directly.</summary>
    Task<List<EmailNotification>> GetRecentNotificationsAsync(int take = 200, CancellationToken cancellationToken = default);
}
