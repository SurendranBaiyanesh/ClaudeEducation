namespace BacklogTicketManager.Logic;

/// <summary>Hashes and verifies passwords. Implementations must use a salted, slow hash.</summary>
public interface IPasswordHasher
{
    /// <summary>Produces a self-describing salted hash string for <paramref name="password"/>.</summary>
    string Hash(string password);

    /// <summary>Constant-time verification of <paramref name="password"/> against a stored hash.</summary>
    bool Verify(string password, string storedHash);
}
