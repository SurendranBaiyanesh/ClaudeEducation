using BacklogTicketManager.Models;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>Read-only audit table of automatically generated emails.</summary>
public partial class EmailLogControl : ComponentBase
{
    [Parameter, EditorRequired]
    public List<EmailNotification> Notifications { get; set; } = new();
}
