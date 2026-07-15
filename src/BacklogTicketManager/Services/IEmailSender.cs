namespace BacklogTicketManager.Services;

/// <summary>
/// Low-level "send this email" abstraction. Kept separate from templating and business
/// rules so the SMTP transport can be swapped or mocked independently (SRP + DIP).
/// </summary>
public interface IEmailSender
{
    Task SendAsync(string toAddress, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
