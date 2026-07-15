using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Pages;

public partial class TicketDetails : ComponentBase
{
    [Parameter]
    public int TicketId { get; set; }

    private Ticket? _ticket;
    private List<TicketUpdate> _updates = new();

    protected override async Task OnParametersSetAsync()
    {
        _ticket = await TicketService.GetTicketAsync(TicketId);
        _updates = _ticket is null ? new List<TicketUpdate>() : await TicketService.GetUpdatesAsync(TicketId);
    }

    private async Task HandleStatusChanged(TicketStatus newStatus)
    {
        await TicketService.ChangeStatusAsync(TicketId, newStatus);
        await OnParametersSetAsync();
    }

    private async Task HandleAddUpdate(string updateText)
    {
        // "Current user" is not modelled by an auth system in this build; the acting user
        // is taken from the ticket's assignee for demonstration purposes.
        var actor = _ticket?.AssignedTo ?? "Unknown";
        await TicketService.AddUpdateAsync(TicketId, updateText, actor);
        _updates = await TicketService.GetUpdatesAsync(TicketId);
    }
}
