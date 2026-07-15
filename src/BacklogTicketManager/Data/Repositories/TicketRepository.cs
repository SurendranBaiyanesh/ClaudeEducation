using System.Data;
using System.Data.Common;
using BacklogTicketManager.Data.Mapping;
using BacklogTicketManager.Data.Schema;
using BacklogTicketManager.Models;
using Microsoft.Data.SqlClient;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>
/// ADO.NET (System.Data) backed repository for tickets. Every query result is loaded into
/// a DataTable whose columns are defined by TicketsTable.xsd (see <see cref="IDataTableSchemaProvider"/>),
/// then converted to <see cref="Ticket"/> instances via <see cref="ITicketMapper"/>.
/// </summary>
public sealed class TicketRepository : ITicketRepository
{
    private const string SelectColumns =
        "Id, Title, Description, Status, Priority, AssignedTo, AssignedToEmail, CreatedDate, DueDate, CompletedDate, LastNotifiedDate";

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDataTableSchemaProvider _schemaProvider;
    private readonly ITicketMapper _mapper;

    public TicketRepository(
        IDbConnectionFactory connectionFactory,
        IDataTableSchemaProvider schemaProvider,
        ITicketMapper mapper)
    {
        _connectionFactory = connectionFactory;
        _schemaProvider = schemaProvider;
        _mapper = mapper;
    }

    public async Task<List<Ticket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var table = await FillTableAsync(
            $"SELECT {SelectColumns} FROM dbo.Tickets ORDER BY DueDate ASC;",
            configureCommand: null,
            cancellationToken);

        return _mapper.FromTable(table);
    }

    public async Task<Ticket?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var table = await FillTableAsync(
            $"SELECT {SelectColumns} FROM dbo.Tickets WHERE Id = @Id;",
            cmd => cmd.Parameters.Add(new SqlParameter("@Id", id)),
            cancellationToken);

        return table.Rows.Count == 0 ? null : _mapper.FromRow(table.Rows[0]);
    }

    public async Task<List<Ticket>> GetOverdueOrDueSoonAsync(TimeSpan dueSoonWindow, CancellationToken cancellationToken = default)
    {
        const string sql = $@"
            SELECT {SelectColumns}
            FROM dbo.Tickets
            WHERE CompletedDate IS NULL
              AND DueDate <= DATEADD(MINUTE, @WindowMinutes, SYSUTCDATETIME())
            ORDER BY DueDate ASC;";

        var table = await FillTableAsync(
            sql,
            cmd => cmd.Parameters.Add(new SqlParameter("@WindowMinutes", (int)dueSoonWindow.TotalMinutes)),
            cancellationToken);

        return _mapper.FromTable(table);
    }

    public async Task<int> InsertAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.Tickets
                (Title, Description, Status, Priority, AssignedTo, AssignedToEmail, CreatedDate, DueDate, CompletedDate, LastNotifiedDate)
            OUTPUT INSERTED.Id
            VALUES
                (@Title, @Description, @Status, @Priority, @AssignedTo, @AssignedToEmail, @CreatedDate, @DueDate, @CompletedDate, @LastNotifiedDate);";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddTicketParameters(command, ticket);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE dbo.Tickets
            SET Title = @Title,
                Description = @Description,
                Status = @Status,
                Priority = @Priority,
                AssignedTo = @AssignedTo,
                AssignedToEmail = @AssignedToEmail,
                DueDate = @DueDate,
                CompletedDate = @CompletedDate,
                LastNotifiedDate = @LastNotifiedDate
            WHERE Id = @Id;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddTicketParameters(command, ticket, includeId: true);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task MarkNotifiedAsync(int ticketId, DateTime notifiedAtUtc, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Tickets SET LastNotifiedDate = @LastNotifiedDate WHERE Id = @Id;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new SqlParameter("@LastNotifiedDate", notifiedAtUtc));
        command.Parameters.Add(new SqlParameter("@Id", ticketId));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM dbo.Tickets WHERE Id = @Id;";
        command.Parameters.Add(new SqlParameter("@Id", id));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Executes <paramref name="sql"/> and loads every row into a DataTable shaped by
    /// TicketsTable.xsd, matching each SQL column to the DataTable column of the same name.
    /// </summary>
    private async Task<DataTable> FillTableAsync(
        string sql,
        Action<DbCommand>? configureCommand,
        CancellationToken cancellationToken)
    {
        var table = _schemaProvider.CreateTicketsTable();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        configureCommand?.Invoke(command);

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

        return table;
    }

    private static void AddTicketParameters(DbCommand command, Ticket ticket, bool includeId = false)
    {
        command.Parameters.Add(new SqlParameter("@Title", ticket.Title));
        command.Parameters.Add(new SqlParameter("@Description", ticket.Description));
        command.Parameters.Add(new SqlParameter("@Status", (int)ticket.Status));
        command.Parameters.Add(new SqlParameter("@Priority", (int)ticket.Priority));
        command.Parameters.Add(new SqlParameter("@AssignedTo", ticket.AssignedTo));
        command.Parameters.Add(new SqlParameter("@AssignedToEmail", ticket.AssignedToEmail));
        command.Parameters.Add(new SqlParameter("@DueDate", ticket.DueDate));
        command.Parameters.Add(new SqlParameter("@CompletedDate", (object?)ticket.CompletedDate ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@LastNotifiedDate", (object?)ticket.LastNotifiedDate ?? DBNull.Value));

        if (includeId)
        {
            command.Parameters.Add(new SqlParameter("@Id", ticket.Id));
        }
        else
        {
            command.Parameters.Add(new SqlParameter("@CreatedDate", ticket.CreatedDate));
        }
    }
}
