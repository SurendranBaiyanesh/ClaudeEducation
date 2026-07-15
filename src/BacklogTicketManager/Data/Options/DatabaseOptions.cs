namespace BacklogTicketManager.Data.Options;

/// <summary>
/// Strongly typed binding for the "ConnectionStrings" configuration section.
/// </summary>
public class DatabaseOptions
{
    public string BacklogDatabase { get; set; } = string.Empty;
}
