using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>
/// Displays a filterable, searchable table of tickets. Contains only presentation/filter
/// state - all data comes from the <see cref="Tickets"/> parameter, keeping the control
/// decoupled from the data access and service layers (single responsibility: rendering).
/// </summary>
public partial class TicketListControl : ComponentBase
{
    [Parameter, EditorRequired]
    public List<Ticket> Tickets { get; set; } = new();

    [Parameter]
    public EventCallback<int> OnSelect { get; set; }

    private static readonly TicketStatus[] StatusOptions = Enum.GetValues<TicketStatus>();

    private string SearchTerm { get; set; } = string.Empty;

    private string StatusFilter { get; set; } = string.Empty;

    private List<Ticket> FilteredTickets => Tickets
        .Where(MatchesSearch)
        .Where(MatchesStatusFilter)
        .OrderBy(t => t.DueDate)
        .ToList();

    private bool MatchesSearch(Ticket ticket)
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
        {
            return true;
        }

        return ticket.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
            || ticket.AssignedTo.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
    }

    private bool MatchesStatusFilter(Ticket ticket) =>
        string.IsNullOrEmpty(StatusFilter) || ticket.Status.ToString() == StatusFilter;

    private void OnSearchChanged(ChangeEventArgs args) => SearchTerm = args.Value?.ToString() ?? string.Empty;

    private void OnStatusFilterChanged(ChangeEventArgs args) => StatusFilter = args.Value?.ToString() ?? string.Empty;
}
