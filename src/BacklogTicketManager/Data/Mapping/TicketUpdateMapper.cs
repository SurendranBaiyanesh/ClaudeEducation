using System.Data;
using BacklogTicketManager.Models;

namespace BacklogTicketManager.Data.Mapping;

public sealed class TicketUpdateMapper : ITicketUpdateMapper
{
    public TicketUpdate FromRow(DataRow row) => new()
    {
        Id = row.Field<int>("Id"),
        TicketId = row.Field<int>("TicketId"),
        UpdateText = row.Field<string?>("UpdateText") ?? string.Empty,
        UpdatedBy = row.Field<string?>("UpdatedBy") ?? string.Empty,
        UpdatedDate = row.Field<DateTime>("UpdatedDate")
    };

    public List<TicketUpdate> FromTable(DataTable table)
    {
        var updates = new List<TicketUpdate>(table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            updates.Add(FromRow(row));
        }
        return updates;
    }
}
