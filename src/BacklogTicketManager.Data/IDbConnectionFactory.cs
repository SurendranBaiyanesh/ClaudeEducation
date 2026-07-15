using System.Data.Common;

namespace BacklogTicketManager.Data;

/// <summary>
/// Creates ADO.NET database connections. Abstracting this behind an interface keeps
/// repositories decoupled from the concrete provider (Dependency Inversion Principle)
/// and makes them unit-testable with a fake connection factory.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>Returns a new, unopened connection to the backlog database.</summary>
    DbConnection CreateConnection();
}
