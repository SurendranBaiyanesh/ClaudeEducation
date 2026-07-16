using System.Data;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Mapping;

public sealed class EmailTemplateMapper : IEmailTemplateMapper
{
    public EmailTemplate FromRow(DataRow row) => new()
    {
        Id = row.Field<int>("Id"),
        Name = row.Field<string?>("Name") ?? string.Empty,
        Description = row.Field<string?>("Description") ?? string.Empty,
        Subject = row.Field<string?>("Subject") ?? string.Empty,
        Body = row.Field<string?>("Body") ?? string.Empty,
        IsActive = row.Field<bool>("IsActive"),
        CreatedDate = row.Field<DateTime>("CreatedDate"),
        UpdatedDate = row.Field<DateTime>("UpdatedDate")
    };

    public List<EmailTemplate> FromTable(DataTable table)
    {
        var templates = new List<EmailTemplate>(table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            templates.Add(FromRow(row));
        }
        return templates;
    }
}
