using System.Data;
using BacklogTicketManager.Data.Mapping;
using BacklogTicketManager.Data.Schema;
using BacklogTicketManager.Models;
using Microsoft.Data.SqlClient;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>
/// ADO.NET repository for the email audit log, using the DataTable shape declared in
/// EmailNotificationsTable.xsd.
/// </summary>
public sealed class EmailNotificationRepository : IEmailNotificationRepository
{
    private const string SelectColumns =
        "Id, TicketId, NotificationType, RecipientEmail, Subject, Body, SentDate, Success, ErrorMessage";

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDataTableSchemaProvider _schemaProvider;
    private readonly IEmailNotificationMapper _mapper;

    public EmailNotificationRepository(
        IDbConnectionFactory connectionFactory,
        IDataTableSchemaProvider schemaProvider,
        IEmailNotificationMapper mapper)
    {
        _connectionFactory = connectionFactory;
        _schemaProvider = schemaProvider;
        _mapper = mapper;
    }

    public async Task<List<EmailNotification>> GetRecentAsync(int take = 200, CancellationToken cancellationToken = default)
    {
        var table = _schemaProvider.CreateEmailNotificationsTable();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT TOP (@Take) {SelectColumns} FROM dbo.EmailNotifications ORDER BY SentDate DESC;";
        command.Parameters.Add(new SqlParameter("@Take", take));

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

    public async Task<int> InsertAsync(EmailNotification notification, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.EmailNotifications
                (TicketId, NotificationType, RecipientEmail, Subject, Body, SentDate, Success, ErrorMessage)
            OUTPUT INSERTED.Id
            VALUES
                (@TicketId, @NotificationType, @RecipientEmail, @Subject, @Body, @SentDate, @Success, @ErrorMessage);";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new SqlParameter("@TicketId", notification.TicketId));
        command.Parameters.Add(new SqlParameter("@NotificationType", (int)notification.NotificationType));
        command.Parameters.Add(new SqlParameter("@RecipientEmail", notification.RecipientEmail));
        command.Parameters.Add(new SqlParameter("@Subject", notification.Subject));
        command.Parameters.Add(new SqlParameter("@Body", notification.Body));
        command.Parameters.Add(new SqlParameter("@SentDate", notification.SentDate));
        command.Parameters.Add(new SqlParameter("@Success", notification.Success));
        command.Parameters.Add(new SqlParameter("@ErrorMessage", (object?)notification.ErrorMessage ?? DBNull.Value));

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<bool> WasNotifiedRecentlyAsync(int ticketId, EmailNotificationType type, TimeSpan window, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT COUNT(1)
            FROM dbo.EmailNotifications
            WHERE TicketId = @TicketId
              AND NotificationType = @NotificationType
              AND Success = 1
              AND SentDate >= DATEADD(MINUTE, -@WindowMinutes, SYSUTCDATETIME());";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new SqlParameter("@TicketId", ticketId));
        command.Parameters.Add(new SqlParameter("@NotificationType", (int)type));
        command.Parameters.Add(new SqlParameter("@WindowMinutes", (int)window.TotalMinutes));

        var count = (int)await command.ExecuteScalarAsync(cancellationToken);
        return count > 0;
    }
}
