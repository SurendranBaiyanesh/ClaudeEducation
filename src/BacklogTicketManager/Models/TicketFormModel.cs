using System.ComponentModel.DataAnnotations;

namespace BacklogTicketManager.Models;

/// <summary>
/// Editable, validated view of a ticket used by <c>TicketFormControl</c>. Kept separate
/// from <see cref="Ticket"/> so validation attributes (a UI concern) never leak into the
/// persistence model (SRP).
/// </summary>
public class TicketFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must be 200 characters or fewer.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000, ErrorMessage = "Description must be 4000 characters or fewer.")]
    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; } = TicketStatus.New;

    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    [Required(ErrorMessage = "Assignee name is required.")]
    public string AssignedTo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Assignee email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string AssignedToEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Due date is required.")]
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);

    public static TicketFormModel FromTicket(Ticket ticket) => new()
    {
        Id = ticket.Id,
        Title = ticket.Title,
        Description = ticket.Description,
        Status = ticket.Status,
        Priority = ticket.Priority,
        AssignedTo = ticket.AssignedTo,
        AssignedToEmail = ticket.AssignedToEmail,
        DueDate = ticket.DueDate
    };

    public Ticket ToTicket() => new()
    {
        Id = Id,
        Title = Title,
        Description = Description,
        Status = Status,
        Priority = Priority,
        AssignedTo = AssignedTo,
        AssignedToEmail = AssignedToEmail,
        DueDate = DueDate
    };
}
