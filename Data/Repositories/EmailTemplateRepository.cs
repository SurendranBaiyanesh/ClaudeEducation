using System.Data;
using System.Data.Common;
using BacklogTicketManager.Data.Mapping;
using BacklogTicketManager.Data.Schema;
using BacklogTicketManager.Common;
using Microsoft.Data.SqlClient;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>
/// ADO.NET (System.Data) backed repository for email templates. Query results are loaded into
/// a DataTable whose columns are defined by EmailTemplatesTable.xsd (see
/// <see cref="IDataTableSchemaProvider"/>), then converted via <see cref="IEmailTemplateMapper"/>.
/// </summary>
public sealed class EmailTemplateRepository : IEmailTemplateRepository
{
    private const string SelectColumns =
        "Id, Name, Description, Subject, Body, IsActive, CreatedDate, UpdatedDate";

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDataTableSchemaProvider _schemaProvider;
    private readonly IEmailTemplateMapper _mapper;

    public EmailTemplateRepository(
        IDbConnectionFactory connectionFactory,
        IDataTableSchemaProvider schemaProvider,
        IEmailTemplateMapper mapper)
    {
        _connectionFactory = connectionFactory;
        _schemaProvider = schemaProvider;
        _mapper = mapper;
    }

    public async Task<List<EmailTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var table = await FillTableAsync(
            $"SELECT {SelectColumns} FROM dbo.EmailTemplates ORDER BY Name ASC;",
            configureCommand: null,
            cancellationToken);

        return _mapper.FromTable(table);
    }

    public async Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var table = await FillTableAsync(
            $"SELECT {SelectColumns} FROM dbo.EmailTemplates WHERE Id = @Id;",
            cmd => cmd.Parameters.Add(new SqlParameter("@Id", id)),
            cancellationToken);

        return table.Rows.Count == 0 ? null : _mapper.FromRow(table.Rows[0]);
    }

    public async Task<int> InsertAsync(EmailTemplate template, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.EmailTemplates
                (Name, Description, Subject, Body, IsActive, CreatedDate, UpdatedDate)
            OUTPUT INSERTED.Id
            VALUES
                (@Name, @Description, @Subject, @Body, @IsActive, @CreatedDate, @UpdatedDate);";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddTemplateParameters(command, template);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(EmailTemplate template, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE dbo.EmailTemplates
            SET Name = @Name,
                Description = @Description,
                Subject = @Subject,
                Body = @Body,
                IsActive = @IsActive,
                UpdatedDate = @UpdatedDate
            WHERE Id = @Id;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddTemplateParameters(command, template, includeId: true);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM dbo.EmailTemplates WHERE Id = @Id;";
        command.Parameters.Add(new SqlParameter("@Id", id));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Executes <paramref name="sql"/> and loads every row into a DataTable shaped by
    /// EmailTemplatesTable.xsd, matching each SQL column to the DataTable column of the same name.
    /// </summary>
    private async Task<DataTable> FillTableAsync(
        string sql,
        Action<DbCommand>? configureCommand,
        CancellationToken cancellationToken)
    {
        var table = _schemaProvider.CreateEmailTemplatesTable();

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

    private static void AddTemplateParameters(DbCommand command, EmailTemplate template, bool includeId = false)
    {
        var now = DateTime.UtcNow;

        command.Parameters.Add(new SqlParameter("@Name", template.Name));
        command.Parameters.Add(new SqlParameter("@Description", template.Description));
        command.Parameters.Add(new SqlParameter("@Subject", template.Subject));
        command.Parameters.Add(new SqlParameter("@Body", template.Body));
        command.Parameters.Add(new SqlParameter("@IsActive", template.IsActive));
        command.Parameters.Add(new SqlParameter("@UpdatedDate", now));

        if (includeId)
        {
            command.Parameters.Add(new SqlParameter("@Id", template.Id));
        }
        else
        {
            command.Parameters.Add(new SqlParameter("@CreatedDate", now));
        }
    }
}
