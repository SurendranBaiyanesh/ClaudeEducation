using System.Data;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Mapping;

public interface IUserMapper
{
    User FromRow(DataRow row);

    List<User> FromTable(DataTable table);
}
