using BacklogTicketManager.Data.Repositories;
using BacklogTicketManager.Common;

namespace BacklogTicketManager.Logic;

/// <summary>
/// Default <see cref="IEmailTemplateService"/> implementation. Depends only on the repository
/// abstraction (DIP); its single reason to change is email-template business rules (SRP).
/// </summary>
public sealed class EmailTemplateService : IEmailTemplateService
{
    private readonly IEmailTemplateRepository _templates;

    public EmailTemplateService(IEmailTemplateRepository templates)
    {
        _templates = templates;
    }

    public Task<List<EmailTemplate>> GetAllAsync() => _templates.GetAllAsync();

    public Task<EmailTemplate?> GetAsync(int id) => _templates.GetByIdAsync(id);

    public Task<int> CreateAsync(EmailTemplate template) => _templates.InsertAsync(template);

    public Task UpdateAsync(EmailTemplate template) => _templates.UpdateAsync(template);

    public Task DeleteAsync(int id) => _templates.DeleteAsync(id);

    public async Task<int> SaveAsync(EmailTemplate template)
    {
        if (template.Id == 0)
        {
            return await _templates.InsertAsync(template);
        }

        await _templates.UpdateAsync(template);
        return template.Id;
    }
}
