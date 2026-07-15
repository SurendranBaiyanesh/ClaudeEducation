using System.Data;
using BacklogTicketManager.Models;

namespace BacklogTicketManager.Data.Mapping;

public interface ITicketUpdateMapper
{
    TicketUpdate FromRow(DataRow row);

    List<TicketUpdate> FromTable(DataTable table);
}
