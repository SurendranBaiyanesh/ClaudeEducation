using System.Data;
using BacklogTicketManager.Models;

namespace BacklogTicketManager.Data.Mapping;

public sealed class EmailNotificationMapper : IEmailNotificationMapper
{
    public EmailNotification FromRow(DataRow row) => new()
    {
        Id = row.Field<int>("Id"),
        TicketId = row.Field<int>("TicketId"),
        NotificationType = (EmailNotificationType)row.Field<int>("NotificationType"),
        RecipientEmail = row.Field<string?>("RecipientEmail") ?? string.Empty,
        Subject = row.Field<string?>("Subject") ?? string.Empty,
        Body = row.Field<string?>("Body") ?? string.Empty,
        SentDate = row.Field<DateTime>("SentDate"),
        Success = row.Field<bool>("Success"),
        ErrorMessage = row.Field<string?>("ErrorMessage")
    };

    public List<EmailNotification> FromTable(DataTable table)
    {
        var notifications = new List<EmailNotification>(table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            notifications.Add(FromRow(row));
        }
        return notifications;
    }
}
