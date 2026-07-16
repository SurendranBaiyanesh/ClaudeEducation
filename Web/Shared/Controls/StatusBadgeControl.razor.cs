using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>Renders a <see cref="TicketStatus"/> as a colour-coded badge. Styling lives in wwwroot/css/statusbadge.css.</summary>
public partial class StatusBadgeControl : ComponentBase
{
    [Parameter, EditorRequired]
    public TicketStatus Status { get; set; }

    private string CssClass => Status switch
    {
        TicketStatus.New => "badge--new",
        TicketStatus.InProgress => "badge--in-progress",
        TicketStatus.Blocked => "badge--blocked",
        TicketStatus.Delayed => "badge--delayed",
        TicketStatus.Completed => "badge--completed",
        TicketStatus.Cancelled => "badge--cancelled",
        _ => "badge--new"
    };
}
