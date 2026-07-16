namespace BacklogTicketManager.Common;

/// <summary>
/// A reusable, named email design stored in the database. Each template holds an HTML body
/// (with {{Placeholder}} tokens) and a subject line that can be rendered for a ticket event.
/// Managed from the Email Templates designer view.
/// </summary>
public class EmailTemplate
{
    public int Id { get; set; }

    /// <summary>Unique, human-friendly identifier for the template (e.g. "Ticket Assigned").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Short note describing when this template is used.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Subject line, may contain {{Placeholder}} tokens.</summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>HTML body, may contain {{Placeholder}} tokens.</summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>Whether the template is available for use.</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
