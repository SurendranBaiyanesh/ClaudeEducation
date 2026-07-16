using BacklogTicketManager.Common;

namespace BacklogTicketManager.Data.Repositories;

/// <summary>Persistence for reusable email templates managed from the designer view.</summary>
public interface IEmailTemplateRepository
{
    Task<List<EmailTemplate>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> InsertAsync(EmailTemplate template, CancellationToken cancellationToken = default);

    Task UpdateAsync(EmailTemplate template, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
