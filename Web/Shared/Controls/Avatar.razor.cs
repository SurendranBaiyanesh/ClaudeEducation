using BacklogTicketManager.Logic;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>Displays the current user's avatar badge (initials), display name and email.
/// Identity comes from <see cref="ICurrentUserContext"/>. Styling lives in wwwroot/css/controls.css.</summary>
public partial class Avatar : ComponentBase
{
    [Inject]
    private ICurrentUserContext User { get; set; } = default!;
}
