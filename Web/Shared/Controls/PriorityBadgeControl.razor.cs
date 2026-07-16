using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>Renders a <see cref="TicketPriority"/> as a colour-coded badge. Styling lives in wwwroot/css/prioritybadge.css.</summary>
public partial class PriorityBadgeControl : ComponentBase
{
    [Parameter, EditorRequired]
    public TicketPriority Priority { get; set; }

    private string CssClass => Priority switch
    {
        TicketPriority.Low => "badge--priority-low",
        TicketPriority.Medium => "badge--priority-medium",
        TicketPriority.High => "badge--priority-high",
        TicketPriority.Critical => "badge--priority-critical",
        _ => "badge--priority-medium"
    };
}
