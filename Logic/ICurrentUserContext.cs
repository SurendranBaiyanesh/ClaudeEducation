namespace BacklogTicketManager.Logic;

/// <summary>Provides the identity of the acting user to the UI layer.
/// A single seam for "who is the current user" so controls (Avatar, and future ones)
/// do not each reach into configuration or an auth system directly.</summary>
public interface ICurrentUserContext
{
    /// <summary>Human-friendly display name (e.g. "MS India").</summary>
    string DisplayName { get; }

    /// <summary>The user's email address.</summary>
    string Email { get; }

    /// <summary>Up-to-two-character initials derived from the display name (or email), for the avatar badge.</summary>
    string Initials { get; }
}
