using BacklogTicketManager.Data.Repositories;
using BacklogTicketManager.Models;
using BacklogTicketManager.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BacklogTicketManager.Services;

/// <summary>
/// Default <see cref="ITicketNotificationService"/> implementation. Every email is logged
/// to <see cref="IEmailNotificationRepository"/> (success or failure) so the Email Log view
/// has a complete audit trail, and so re-notification can be throttled.
/// </summary>
public sealed class TicketNotificationService : ITicketNotificationService
{
    private readonly ITicketRepository _tickets;
    private readonly IEmailNotificationRepository _notificationLog;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateBuilder _templateBuilder;
    private readonly NotificationOptions _options;
    private readonly ILogger<TicketNotificationService> _logger;

    public TicketNotificationService(
        ITicketRepository tickets,
        IEmailNotificationRepository notificationLog,
        IEmailSender emailSender,
        IEmailTemplateBuilder templateBuilder,
        IOptions<NotificationOptions> options,
        ILogger<TicketNotificationService> logger)
    {
        _tickets = tickets;
        _notificationLog = notificationLog;
        _emailSender = emailSender;
        _templateBuilder = templateBuilder;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<int> ProcessDueAndOverdueTicketsAsync(CancellationToken cancellationToken = default)
    {
        var dueSoonWindow = TimeSpan.FromHours(_options.DueSoonWindowHours);
        var renotifyWindow = TimeSpan.FromHours(_options.RenotifyAfterHours);

        var candidates = await _tickets.GetOverdueOrDueSoonAsync(dueSoonWindow, cancellationToken);
        var sentCount = 0;

        foreach (var ticket in candidates)
        {
            var type = ticket.IsOverdue ? EmailNotificationType.TaskDelayed : EmailNotificationType.DeadlineApproaching;

            var alreadyNotified = await _notificationLog.WasNotifiedRecentlyAsync(ticket.Id, type, renotifyWindow, cancellationToken);
            if (alreadyNotified)
            {
                continue;
            }

            var (subject, body) = type == EmailNotificationType.TaskDelayed
                ? _templateBuilder.BuildDelayedEmail(ticket)
                : _templateBuilder.BuildDueSoonEmail(ticket);

            var sent = await SendAndLogAsync(ticket, type, subject, body, cancellationToken);
            if (sent)
            {
                await _tickets.MarkNotifiedAsync(ticket.Id, DateTime.UtcNow, cancellationToken);
                sentCount++;
            }
        }

        return sentCount;
    }

    public async Task NotifyStatusChangedAsync(Ticket ticket, TicketStatus previousStatus, CancellationToken cancellationToken = default)
    {
        var (subject, body) = _templateBuilder.BuildStatusChangedEmail(ticket, previousStatus);
        await SendAndLogAsync(ticket, EmailNotificationType.StatusChanged, subject, body, cancellationToken);
    }

    public Task<List<EmailNotification>> GetRecentNotificationsAsync(int take = 200, CancellationToken cancellationToken = default) =>
        _notificationLog.GetRecentAsync(take, cancellationToken);

    private async Task<bool> SendAndLogAsync(
        Ticket ticket,
        EmailNotificationType type,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        var recipient = string.IsNullOrWhiteSpace(ticket.AssignedToEmail)
            ? _options.DefaultRecipient
            : ticket.AssignedToEmail;

        var notification = new EmailNotification
        {
            TicketId = ticket.Id,
            NotificationType = type,
            RecipientEmail = recipient,
            Subject = subject,
            Body = body,
            SentDate = DateTime.UtcNow
        };

        try
        {
            await _emailSender.SendAsync(recipient, subject, body, cancellationToken);
            notification.Success = true;
        }
        catch (Exception ex)
        {
            notification.Success = false;
            notification.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Failed to send {NotificationType} email for ticket {TicketId}", type, ticket.Id);
        }

        await _notificationLog.InsertAsync(notification, cancellationToken);
        return notification.Success;
    }
}
