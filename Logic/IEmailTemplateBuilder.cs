using BacklogTicketManager.Common;

namespace BacklogTicketManager.Logic;

/// <summary>Builds the subject/body pair for each type of automatic notification email.</summary>
public interface IEmailTemplateBuilder
{
    (string Subject, string HtmlBody) BuildDelayedEmail(Ticket ticket);

    (string Subject, string HtmlBody) BuildDueSoonEmail(Ticket ticket);

    (string Subject, string HtmlBody) BuildStatusChangedEmail(Ticket ticket, TicketStatus previousStatus);
}
