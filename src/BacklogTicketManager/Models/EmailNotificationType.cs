namespace BacklogTicketManager.Models;

/// <summary>
/// The event that caused an automatic email to be generated.
/// </summary>
public enum EmailNotificationType
{
    TaskDelayed = 0,
    DeadlineApproaching = 1,
    StatusChanged = 2,
    TicketAssigned = 3
}
