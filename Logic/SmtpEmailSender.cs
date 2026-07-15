using System.Net;
using System.Net.Mail;
using BacklogTicketManager.Logic.Options;
using Microsoft.Extensions.Options;

namespace BacklogTicketManager.Logic;

/// <summary>
/// Sends email via SMTP using <see cref="System.Net.Mail.SmtpClient"/>, configured from
/// the "Smtp" section of appsettings.json.
/// </summary>
public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public SmtpEmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string toAddress, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.UserName, _options.Password)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromDisplayName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(toAddress);

        cancellationToken.ThrowIfCancellationRequested();
        await client.SendMailAsync(message, cancellationToken);
    }
}
