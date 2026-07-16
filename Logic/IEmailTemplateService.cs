using BacklogTicketManager.Common;

namespace BacklogTicketManager.Logic;

/// <summary>
/// Application-level email-template operations consumed by the designer view. Wraps the
/// repository so pages never talk to Data.Repositories directly.
/// </summary>
public interface IEmailTemplateService
{
    Task<List<EmailTemplate>> GetAllAsync();

    Task<EmailTemplate?> GetAsync(int id);

    Task<int> CreateAsync(EmailTemplate template);

    Task UpdateAsync(EmailTemplate template);

    Task DeleteAsync(int id);

    /// <summary>Creates the template when Id is 0, otherwise updates it. Returns the template Id.</summary>
    Task<int> SaveAsync(EmailTemplate template);
}
