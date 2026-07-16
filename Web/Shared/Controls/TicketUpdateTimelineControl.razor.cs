using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>
/// Displays a ticket's update timeline and a small composer for adding a new entry.
/// Persistence is delegated to the caller via <see cref="OnAddUpdate"/>.
/// </summary>
public partial class TicketUpdateTimelineControl : ComponentBase
{
    [Parameter, EditorRequired]
    public List<TicketUpdate> Updates { get; set; } = new();

    [Parameter]
    public EventCallback<string> OnAddUpdate { get; set; }

    private string NewUpdateText { get; set; } = string.Empty;

    private async Task SubmitUpdate()
    {
        if (string.IsNullOrWhiteSpace(NewUpdateText))
        {
            return;
        }

        await OnAddUpdate.InvokeAsync(NewUpdateText);
        NewUpdateText = string.Empty;
    }
}
