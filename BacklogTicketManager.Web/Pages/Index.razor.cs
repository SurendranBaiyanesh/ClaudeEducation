using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private List<Ticket>? _tickets;

    protected override async Task OnInitializedAsync()
    {
        _tickets = await TicketService.GetAllTicketsAsync();
    }

    private void GoToTicket(int ticketId) => Navigation.NavigateTo($"tickets/{ticketId}");
}
