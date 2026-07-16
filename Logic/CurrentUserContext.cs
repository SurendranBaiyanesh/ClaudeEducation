using BacklogTicketManager.Logic.Options;
using Microsoft.Extensions.Options;

namespace BacklogTicketManager.Logic;

/// <summary>Configuration-backed <see cref="ICurrentUserContext"/>.
/// Reads the "CurrentUser" section; falls back to the email local-part when no display name is set.</summary>
public class CurrentUserContext : ICurrentUserContext
{
    private readonly CurrentUserOptions _options;

    public CurrentUserContext(IOptions<CurrentUserOptions> options)
    {
        _options = options.Value;
    }

    public string DisplayName =>
        !string.IsNullOrWhiteSpace(_options.DisplayName)
            ? _options.DisplayName
            : LocalPart(_options.Email);

    public string Email => _options.Email;

    public string Initials => DeriveInitials(DisplayName, _options.Email);

    /// <summary>Takes the part of an email before the '@' (used as a display-name fallback).</summary>
    private static string LocalPart(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return "User";
        }

        var at = email.IndexOf('@');
        return at > 0 ? email[..at] : email;
    }

    /// <summary>Derives up to two uppercase initials from the display name, falling back to the email.</summary>
    private static string DeriveInitials(string displayName, string email)
    {
        var source = !string.IsNullOrWhiteSpace(displayName) ? displayName : LocalPart(email);

        var words = source.Split(new[] { ' ', '.', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            return "?";
        }

        if (words.Length == 1)
        {
            var word = words[0];
            return (word.Length >= 2 ? word[..2] : word).ToUpperInvariant();
        }

        return $"{words[0][0]}{words[^1][0]}".ToUpperInvariant();
    }
}
