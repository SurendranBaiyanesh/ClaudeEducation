namespace BacklogTicketManager.Logic.Options;

/// <summary>Strongly typed binding for the "CurrentUser" configuration section.
/// This app has no authentication system, so the acting user is supplied via configuration.</summary>
public class CurrentUserOptions
{
    /// <summary>Display name shown next to the avatar.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Email address shown under the display name and used to derive the avatar.</summary>
    public string Email { get; set; } = string.Empty;
}
