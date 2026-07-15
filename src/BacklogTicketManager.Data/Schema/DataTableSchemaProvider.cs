using System.Data;

namespace BacklogTicketManager.Data.Schema;

/// <summary>
/// Loads each table's structure from its XML schema (.xsd) file via
/// <see cref="DataSet.ReadXmlSchema(string)"/>. Schemas are read once and cached as
/// templates; callers get back a <see cref="DataTable.Clone"/> so repositories can safely
/// fill and mutate their own copy without interfering with other requests.
/// </summary>
public sealed class DataTableSchemaProvider : IDataTableSchemaProvider
{
    private readonly string _schemaDirectory;
    private readonly Lazy<DataTable> _ticketsTemplate;
    private readonly Lazy<DataTable> _ticketUpdatesTemplate;
    private readonly Lazy<DataTable> _emailNotificationsTemplate;

    public DataTableSchemaProvider()
    {
        _schemaDirectory = Path.Combine(AppContext.BaseDirectory, "Schema", "Xml");

        _ticketsTemplate = new Lazy<DataTable>(() =>
            LoadTable("TicketsTable.xsd", "Ticket"));

        _ticketUpdatesTemplate = new Lazy<DataTable>(() =>
            LoadTable("TicketUpdatesTable.xsd", "TicketUpdate"));

        _emailNotificationsTemplate = new Lazy<DataTable>(() =>
            LoadTable("EmailNotificationsTable.xsd", "EmailNotification"));
    }

    public DataTable CreateTicketsTable() => _ticketsTemplate.Value.Clone();

    public DataTable CreateTicketUpdatesTable() => _ticketUpdatesTemplate.Value.Clone();

    public DataTable CreateEmailNotificationsTable() => _emailNotificationsTemplate.Value.Clone();

    private DataTable LoadTable(string xsdFileName, string tableElementName)
    {
        var path = Path.Combine(_schemaDirectory, xsdFileName);
        var dataSet = new DataSet();
        dataSet.ReadXmlSchema(path);

        if (!dataSet.Tables.Contains(tableElementName))
        {
            throw new InvalidOperationException(
                $"XML schema '{xsdFileName}' does not define an element named '{tableElementName}'.");
        }

        return dataSet.Tables[tableElementName]!;
    }
}
