namespace BacklogTicketManager.Services.Options;

/// <summary>Strongly typed binding for the "Notification" configuration section.</summary>
public class NotificationOptions
{
    /// <summary>How often the background monitor scans for delayed / due-soon tickets.</summary>
    public int PollingIntervalMinutes { get; set; } = 15;

    /// <summary>How far ahead of the due date a "deadline approaching" email is sent.</summary>
    public int DueSoonWindowHours { get; set; } = 24;

    /// <summary>Minimum gap before the same notification type can be re-sent for a ticket.</summary>
    public int RenotifyAfterHours { get; set; } = 24;

    /// <summary>Fallback recipient used when a ticket has no assignee email.</summary>
    public string DefaultRecipient { get; set; } = string.Empty;
}
