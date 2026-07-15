using System.Data;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Mapping;

public interface ITicketUpdateMapper
{
    TicketUpdate FromRow(DataRow row);

    List<TicketUpdate> FromTable(DataTable table);
}
