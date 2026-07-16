using System.Data;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Mapping;

public interface IEmailTemplateMapper
{
    EmailTemplate FromRow(DataRow row);

    List<EmailTemplate> FromTable(DataTable table);
}
