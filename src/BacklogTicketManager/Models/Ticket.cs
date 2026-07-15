namespace BacklogTicketManager.Models;

/// <summary>
/// A single backlog ticket. This is a plain data holder (POCO) - it has no persistence
/// or email logic, keeping it free of responsibilities that belong to the Data/Services layers.
/// </summary>
public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public string AssignedTo { get; set; } = string.Empty;
    public string AssignedToEmail { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? LastNotifiedDate { get; set; }

    /// <summary>True once the ticket's due date has passed without completion.</summary>
    public bool IsOverdue => CompletedDate is null && DateTime.UtcNow > DueDate;

    /// <summary>True when the ticket is due within the supplied window but not yet overdue.</summary>
    public bool IsDueSoon(TimeSpan window) =>
        CompletedDate is null && !IsOverdue && DateTime.UtcNow >= DueDate - window;
}
