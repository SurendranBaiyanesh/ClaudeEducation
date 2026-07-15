using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Pages;

public partial class NewTicket : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private readonly TicketFormModel _model = new();

    private async Task HandleSubmit(TicketFormModel model)
    {
        var ticket = model.ToTicket();
        var id = await TicketService.CreateTicketAsync(ticket);
        Navigation.NavigateTo($"tickets/{id}");
    }
}
