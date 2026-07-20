using BacklogTicketManager.Common;

namespace BacklogTicketManager.Logic;

/// <summary>Outcome of a registration attempt.</summary>
public sealed record RegisterResult(bool Succeeded, string? Error, User? User)
{
    public static RegisterResult Success(User user) => new(true, null, user);
    public static RegisterResult Failure(string error) => new(false, error, null);
}

/// <summary>
/// Application-level authentication operations: creating accounts and validating credentials.
/// Passwords are hashed via <see cref="IPasswordHasher"/>; this service never stores or
/// returns raw passwords.
/// </summary>
public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterModel model);

    /// <summary>Returns the user when the username/password are valid; otherwise null.</summary>
    Task<User?> ValidateCredentialsAsync(string username, string password);

    /// <summary>Seeds a default admin account when no users exist yet (idempotent).</summary>
    Task EnsureDefaultAdminAsync(string username, string email, string displayName, string password);
}
