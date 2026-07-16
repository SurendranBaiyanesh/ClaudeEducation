using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>
/// Shows a ticket's due date and flags whether it is overdue or due soon. Styling lives in
/// wwwroot/css/deadlineindicator.css.
/// </summary>
public partial class DeadlineIndicatorControl : ComponentBase
{
    private static readonly TimeSpan DueSoonWindow = TimeSpan.FromHours(24);

    [Parameter, EditorRequired]
    public Ticket Ticket { get; set; } = null!;

    private string CssClass => Ticket switch
    {
        { IsOverdue: true } => "deadline-indicator--overdue",
        _ when Ticket.IsDueSoon(DueSoonWindow) => "deadline-indicator--due-soon",
        _ => "deadline-indicator--on-track"
    };

    private string Label => Ticket switch
    {
        { IsOverdue: true } => "Overdue",
        _ when Ticket.IsDueSoon(DueSoonWindow) => "Due soon",
        _ => string.Empty
    };
}
