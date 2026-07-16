using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>Persistence/audit trail for automatic emails sent by the system.</summary>
public interface IEmailNotificationRepository
{
    Task<List<EmailNotification>> GetRecentAsync(int take = 200, CancellationToken cancellationToken = default);

    Task<int> InsertAsync(EmailNotification notification, CancellationToken cancellationToken = default);

    /// <summary>True if a notification of this type was already sent for the ticket within the window.</summary>
    Task<bool> WasNotifiedRecentlyAsync(int ticketId, EmailNotificationType type, TimeSpan window, CancellationToken cancellationToken = default);
}
