using System.Net;
using BacklogTicketManager.Models;

namespace BacklogTicketManager.Services;

/// <summary>
/// Produces simple, self-contained HTML email bodies. Templates are inline strings rather
/// than Razor views to keep the email pipeline free of any UI-framework dependency.
/// </summary>
public sealed class EmailTemplateBuilder : IEmailTemplateBuilder
{
    public (string Subject, string HtmlBody) BuildDelayedEmail(Ticket ticket)
    {
        var subject = $"[Backlog] Ticket #{ticket.Id} is delayed: {ticket.Title}";
        var body = Wrap(
            "Ticket Delayed",
            $"""
             <p>Ticket <strong>#{ticket.Id} - {Encode(ticket.Title)}</strong> has passed its due date and has not been completed.</p>
             <table class="email-table">
               <tr><td>Assigned to</td><td>{Encode(ticket.AssignedTo)}</td></tr>
               <tr><td>Due date</td><td>{ticket.DueDate:yyyy-MM-dd HH:mm} UTC</td></tr>
               <tr><td>Status</td><td>{ticket.Status}</td></tr>
             </table>
             <p>Please update the ticket status or extend the deadline.</p>
             """);

        return (subject, body);
    }

    public (string Subject, string HtmlBody) BuildDueSoonEmail(Ticket ticket)
    {
        var subject = $"[Backlog] Ticket #{ticket.Id} is due soon: {ticket.Title}";
        var body = Wrap(
            "Deadline Approaching",
            $"""
             <p>Ticket <strong>#{ticket.Id} - {Encode(ticket.Title)}</strong> is due soon.</p>
             <table class="email-table">
               <tr><td>Assigned to</td><td>{Encode(ticket.AssignedTo)}</td></tr>
               <tr><td>Due date</td><td>{ticket.DueDate:yyyy-MM-dd HH:mm} UTC</td></tr>
               <tr><td>Status</td><td>{ticket.Status}</td></tr>
             </table>
             """);

        return (subject, body);
    }

    public (string Subject, string HtmlBody) BuildStatusChangedEmail(Ticket ticket, TicketStatus previousStatus)
    {
        var subject = $"[Backlog] Ticket #{ticket.Id} status changed: {ticket.Title}";
        var body = Wrap(
            "Status Changed",
            $"""
             <p>Ticket <strong>#{ticket.Id} - {Encode(ticket.Title)}</strong> changed status.</p>
             <table class="email-table">
               <tr><td>Previous status</td><td>{previousStatus}</td></tr>
               <tr><td>New status</td><td>{ticket.Status}</td></tr>
               <tr><td>Assigned to</td><td>{Encode(ticket.AssignedTo)}</td></tr>
             </table>
             """);

        return (subject, body);
    }

    private static string Encode(string value) => WebUtility.HtmlEncode(value);

    private static string Wrap(string heading, string innerHtml) =>
        $"""
         <html>
           <body style="font-family:Segoe UI,Arial,sans-serif;color:#1f2933;">
             <h2>{heading}</h2>
             {innerHtml}
             <p style="color:#6b7280;font-size:12px;">This is an automated message from Backlog Ticket Manager.</p>
           </body>
         </html>
         """;
}
