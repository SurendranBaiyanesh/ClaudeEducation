using System.Data;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Mapping;

/// <summary>
/// Translates between the XML-shaped Tickets <see cref="DataTable"/> and the strongly
/// typed <see cref="Ticket"/> model. Isolating this mapping keeps the repository focused
/// on data access and the model free of any ADO.NET/DataTable concerns (SRP).
/// </summary>
public interface ITicketMapper
{
    Ticket FromRow(DataRow row);

    List<Ticket> FromTable(DataTable table);
}
