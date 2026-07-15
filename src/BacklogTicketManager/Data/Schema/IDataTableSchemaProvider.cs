using System.Data;

namespace BacklogTicketManager.Data.Schema;

/// <summary>
/// Produces empty, strongly-shaped <see cref="DataTable"/> instances whose columns (names,
/// types, keys) are defined entirely by XML schema files under Data/Schema/Xml. Repositories
/// fill these tables with SQL query results; nothing in the codebase hand-declares a
/// DataColumn - the XML file is the single source of truth for each table's field names.
/// </summary>
public interface IDataTableSchemaProvider
{
    /// <summary>Empty DataTable shaped from Data/Schema/Xml/TicketsTable.xsd.</summary>
    DataTable CreateTicketsTable();

    /// <summary>Empty DataTable shaped from Data/Schema/Xml/TicketUpdatesTable.xsd.</summary>
    DataTable CreateTicketUpdatesTable();

    /// <summary>Empty DataTable shaped from Data/Schema/Xml/EmailNotificationsTable.xsd.</summary>
    DataTable CreateEmailNotificationsTable();
}
