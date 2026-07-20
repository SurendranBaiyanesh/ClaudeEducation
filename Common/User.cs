namespace BacklogTicketManager.Common;

/// <summary>
/// An application user who can sign in. The password is never stored in the clear -
/// <see cref="PasswordHash"/> holds a salted PBKDF2 hash produced by the Logic layer.
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Salted PBKDF2 hash (format: iterations.saltBase64.hashBase64). Never the raw password.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime? LastLoginDate { get; set; }
}
