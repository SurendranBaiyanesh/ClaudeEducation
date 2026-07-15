using System.Data.Common;
using BacklogTicketManager.Data.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace BacklogTicketManager.Data;

/// <summary>
/// Creates <see cref="SqlConnection"/> instances (System.Data.Common.DbConnection) using the
/// connection string bound from configuration. This is the single place that knows the
/// concrete ADO.NET provider - every other class depends only on <see cref="IDbConnectionFactory"/>.
/// </summary>
public sealed class SqlDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlDbConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _connectionString = options.Value.BacklogDatabase;
    }

    public DbConnection CreateConnection() => new SqlConnection(_connectionString);
}
