using BacklogTicketManager.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>
/// Read-only display of a ticket's fields plus a status-change control. Raises
/// <see cref="OnStatusChanged"/> and leaves persistence/email side effects to the caller.
/// </summary>
public partial class TicketDetailControl : ComponentBase
{
    [Parameter, EditorRequired]
    public Ticket Ticket { get; set; } = null!;

    [Parameter]
    public EventCallback<TicketStatus> OnStatusChanged { get; set; }

    private Task HandleStatusChanged(ChangeEventArgs args)
    {
        if (Enum.TryParse<TicketStatus>(args.Value?.ToString(), out var newStatus))
        {
            return OnStatusChanged.InvokeAsync(newStatus);
        }

        return Task.CompletedTask;
    }
}
