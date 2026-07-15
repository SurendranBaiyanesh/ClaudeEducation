namespace BacklogTicketManager.Models;

/// <summary>
/// A record of an automatic email that was generated for a ticket event, kept for
/// auditing in the Email Log view and to avoid re-sending duplicate notifications.
/// </summary>
public class EmailNotification
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public EmailNotificationType NotificationType { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
