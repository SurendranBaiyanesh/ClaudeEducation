using BacklogTicketManager.Models;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Pages;

public partial class EmailLog : ComponentBase
{
    private List<EmailNotification>? _notifications;

    protected override async Task OnInitializedAsync()
    {
        _notifications = await NotificationService.GetRecentNotificationsAsync();
    }
}
