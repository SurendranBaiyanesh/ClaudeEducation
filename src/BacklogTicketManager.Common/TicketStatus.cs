namespace BacklogTicketManager.Common;

/// <summary>
/// Lifecycle states for a backlog ticket.
/// </summary>
public enum TicketStatus
{
    New = 0,
    InProgress = 1,
    Blocked = 2,
    Delayed = 3,
    Completed = 4,
    Cancelled = 5
}
