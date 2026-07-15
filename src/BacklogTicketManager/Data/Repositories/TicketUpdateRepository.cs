using System.Data;
using BacklogTicketManager.Data.Mapping;
using BacklogTicketManager.Data.Schema;
using BacklogTicketManager.Models;
using Microsoft.Data.SqlClient;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>
/// ADO.NET repository for ticket timeline entries, using the DataTable shape declared in
/// TicketUpdatesTable.xsd.
/// </summary>
public sealed class TicketUpdateRepository : ITicketUpdateRepository
{
    private const string SelectColumns = "Id, TicketId, [UpdateText], UpdatedBy, UpdatedDate";

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDataTableSchemaProvider _schemaProvider;
    private readonly ITicketUpdateMapper _mapper;

    public TicketUpdateRepository(
        IDbConnectionFactory connectionFactory,
        IDataTableSchemaProvider schemaProvider,
        ITicketUpdateMapper mapper)
    {
        _connectionFactory = connectionFactory;
        _schemaProvider = schemaProvider;
        _mapper = mapper;
    }

    public async Task<List<TicketUpdate>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default)
    {
        var table = _schemaProvider.CreateTicketUpdatesTable();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {SelectColumns} FROM dbo.TicketUpdates WHERE TicketId = @TicketId ORDER BY UpdatedDate DESC;";
        command.Parameters.Add(new SqlParameter("@TicketId", ticketId));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var row = table.NewRow();
            foreach (DataColumn column in table.Columns)
            {
                var ordinal = reader.GetOrdinal(column.ColumnName);
                row[column] = reader.IsDBNull(ordinal) ? DBNull.Value : reader.GetValue(ordinal);
            }
            table.Rows.Add(row);
        }

        return _mapper.FromTable(table);
    }

    public async Task<int> InsertAsync(TicketUpdate update, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.TicketUpdates (TicketId, [UpdateText], UpdatedBy, UpdatedDate)
            OUTPUT INSERTED.Id
            VALUES (@TicketId, @UpdateText, @UpdatedBy, @UpdatedDate);";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new SqlParameter("@TicketId", update.TicketId));
        command.Parameters.Add(new SqlParameter("@UpdateText", update.UpdateText));
        command.Parameters.Add(new SqlParameter("@UpdatedBy", update.UpdatedBy));
        command.Parameters.Add(new SqlParameter("@UpdatedDate", update.UpdatedDate));

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }
}
