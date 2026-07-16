using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Pages;

/// <summary>
/// Email template designer: lists saved templates and edits the selected one (or a new one)
/// through <see cref="Shared.Controls.EmailTemplateEditorControl"/>. Persists via
/// <see cref="Logic.IEmailTemplateService"/>.
/// </summary>
public partial class EmailTemplates : ComponentBase
{
    private List<EmailTemplate>? _templates;
    private EmailTemplateFormModel _model = new();
    private string? _statusMessage;

    protected override async Task OnInitializedAsync()
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        _templates = await TemplateService.GetAllAsync();
    }

    private void StartNew()
    {
        _model = new EmailTemplateFormModel();
        _statusMessage = null;
    }

    private async Task SelectTemplate(int id)
    {
        var template = await TemplateService.GetAsync(id);
        if (template is not null)
        {
            _model = EmailTemplateFormModel.FromTemplate(template);
            _statusMessage = null;
        }
    }

    private async Task HandleSave(EmailTemplateFormModel model)
    {
        var id = await TemplateService.SaveAsync(model.ToTemplate());
        await ReloadAsync();

        var saved = await TemplateService.GetAsync(id);
        if (saved is not null)
        {
            _model = EmailTemplateFormModel.FromTemplate(saved);
        }

        _statusMessage = $"Template \"{model.Name}\" saved.";
    }

    private async Task HandleDelete(EmailTemplateFormModel model)
    {
        if (model.Id == 0)
        {
            return;
        }

        await TemplateService.DeleteAsync(model.Id);
        await ReloadAsync();
        _model = new EmailTemplateFormModel();
        _statusMessage = $"Template \"{model.Name}\" deleted.";
    }
}
