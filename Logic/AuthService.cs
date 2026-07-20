using BacklogTicketManager.Data.Repositories;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Logic;

/// <summary>
/// Default <see cref="IAuthService"/> implementation. Depends only on the user repository and
/// password hasher (DIP); enforces account rules (unique username/email, password hashing).
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResult> RegisterAsync(RegisterModel model)
    {
        var username = model.Username.Trim();
        var email = model.Email.Trim();

        if (await _users.ExistsAsync(username, email))
        {
            return RegisterResult.Failure("That username or email is already registered.");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            DisplayName = model.DisplayName.Trim(),
            PasswordHash = _passwordHasher.Hash(model.Password),
            CreatedDate = DateTime.UtcNow
        };

        user.Id = await _users.InsertAsync(user);
        return RegisterResult.Success(user);
    }

    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _users.GetByUsernameAsync(username.Trim());
        if (user is null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            return null;
        }

        await _users.UpdateLastLoginAsync(user.Id, DateTime.UtcNow);
        return user;
    }

    public async Task EnsureDefaultAdminAsync(string username, string email, string displayName, string password)
    {
        if (await _users.CountAsync() > 0)
        {
            return;
        }

        var admin = new User
        {
            Username = username,
            Email = email,
            DisplayName = displayName,
            PasswordHash = _passwordHasher.Hash(password),
            CreatedDate = DateTime.UtcNow
        };

        await _users.InsertAsync(admin);
    }
}
