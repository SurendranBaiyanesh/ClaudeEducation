using System.Data;
using BacklogTicketManager.Models;

namespace BacklogTicketManager.Data.Mapping;

public sealed class TicketMapper : ITicketMapper
{
    public Ticket FromRow(DataRow row) => new()
    {
        Id = row.Field<int>("Id"),
        Title = row.Field<string?>("Title") ?? string.Empty,
        Description = row.Field<string?>("Description") ?? string.Empty,
        Status = (TicketStatus)row.Field<int>("Status"),
        Priority = (TicketPriority)row.Field<int>("Priority"),
        AssignedTo = row.Field<string?>("AssignedTo") ?? string.Empty,
        AssignedToEmail = row.Field<string?>("AssignedToEmail") ?? string.Empty,
        CreatedDate = row.Field<DateTime>("CreatedDate"),
        DueDate = row.Field<DateTime>("DueDate"),
        CompletedDate = row.Field<DateTime?>("CompletedDate"),
        LastNotifiedDate = row.Field<DateTime?>("LastNotifiedDate")
    };

    public List<Ticket> FromTable(DataTable table)
    {
        var tickets = new List<Ticket>(table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            tickets.Add(FromRow(row));
        }
        return tickets;
    }
}
