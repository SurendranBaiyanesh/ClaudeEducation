using System.Data;
using BacklogTicketManager.Models;

namespace BacklogTicketManager.Data.Mapping;

public interface IEmailNotificationMapper
{
    EmailNotification FromRow(DataRow row);

    List<EmailNotification> FromTable(DataTable table);
}
