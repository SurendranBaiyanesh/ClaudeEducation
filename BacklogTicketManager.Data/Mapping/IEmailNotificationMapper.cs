using System.Data;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Mapping;

public interface IEmailNotificationMapper
{
    EmailNotification FromRow(DataRow row);

    List<EmailNotification> FromTable(DataTable table);
}
