using System.Data;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Mapping;

public sealed class UserMapper : IUserMapper
{
    public User FromRow(DataRow row) => new()
    {
        Id = row.Field<int>("Id"),
        Username = row.Field<string?>("Username") ?? string.Empty,
        Email = row.Field<string?>("Email") ?? string.Empty,
        DisplayName = row.Field<string?>("DisplayName") ?? string.Empty,
        PasswordHash = row.Field<string?>("PasswordHash") ?? string.Empty,
        CreatedDate = row.Field<DateTime>("CreatedDate"),
        LastLoginDate = row.Field<DateTime?>("LastLoginDate")
    };

    public List<User> FromTable(DataTable table)
    {
        var users = new List<User>(table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            users.Add(FromRow(row));
        }
        return users;
    }
}
