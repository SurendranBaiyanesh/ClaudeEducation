namespace BacklogTicketManager.Common;

/// <summary>
/// A single timeline entry (comment / progress note) attached to a ticket.
/// </summary>
public class TicketUpdate
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string UpdateText { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
}
