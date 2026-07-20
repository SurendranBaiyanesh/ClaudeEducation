using System.Data;
using System.Data.Common;
using BacklogTicketManager.Data.Mapping;
using BacklogTicketManager.Data.Schema;
using BacklogTicketManager.Common;
using Microsoft.Data.SqlClient;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>
/// ADO.NET (System.Data) backed repository for users. Query results are loaded into a
/// DataTable shaped by UsersTable.xsd (see <see cref="IDataTableSchemaProvider"/>), then
/// converted via <see cref="IUserMapper"/>.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private const string SelectColumns =
        "Id, Username, Email, DisplayName, PasswordHash, CreatedDate, LastLoginDate";

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDataTableSchemaProvider _schemaProvider;
    private readonly IUserMapper _mapper;

    public UserRepository(
        IDbConnectionFactory connectionFactory,
        IDataTableSchemaProvider schemaProvider,
        IUserMapper mapper)
    {
        _connectionFactory = connectionFactory;
        _schemaProvider = schemaProvider;
        _mapper = mapper;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var table = await FillTableAsync(
            $"SELECT {SelectColumns} FROM dbo.Users WHERE Username = @Username;",
            cmd => cmd.Parameters.Add(new SqlParameter("@Username", username)),
            cancellationToken);

        return table.Rows.Count == 0 ? null : _mapper.FromRow(table.Rows[0]);
    }

    public async Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Users WHERE Username = @Username OR Email = @Email;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new SqlParameter("@Username", username));
        command.Parameters.Add(new SqlParameter("@Email", email));

        var count = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        return count > 0;
    }

    public async Task<int> InsertAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.Users
                (Username, Email, DisplayName, PasswordHash, CreatedDate, LastLoginDate)
            OUTPUT INSERTED.Id
            VALUES
                (@Username, @Email, @DisplayName, @PasswordHash, @CreatedDate, @LastLoginDate);";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new SqlParameter("@Username", user.Username));
        command.Parameters.Add(new SqlParameter("@Email", user.Email));
        command.Parameters.Add(new SqlParameter("@DisplayName", user.DisplayName));
        command.Parameters.Add(new SqlParameter("@PasswordHash", user.PasswordHash));
        command.Parameters.Add(new SqlParameter("@CreatedDate", user.CreatedDate));
        command.Parameters.Add(new SqlParameter("@LastLoginDate", (object?)user.LastLoginDate ?? DBNull.Value));

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task UpdateLastLoginAsync(int userId, DateTime whenUtc, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dbo.Users SET LastLoginDate = @When WHERE Id = @Id;";
        command.Parameters.Add(new SqlParameter("@When", whenUtc));
        command.Parameters.Add(new SqlParameter("@Id", userId));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM dbo.Users;";

        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
    }

    private async Task<DataTable> FillTableAsync(
        string sql,
        Action<DbCommand>? configureCommand,
        CancellationToken cancellationToken)
    {
        var table = _schemaProvider.CreateUsersTable();

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
}
