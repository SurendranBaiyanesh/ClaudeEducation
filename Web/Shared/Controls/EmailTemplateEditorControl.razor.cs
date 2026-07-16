using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>
/// Create/edit form for an email template with a live HTML preview. Validation is declarative
/// (DataAnnotations on <see cref="EmailTemplateFormModel"/>); this control only raises
/// <see cref="OnSubmit"/> / <see cref="OnDelete"/> - it never talks to services directly.
/// Styling lives in wwwroot/css/emailtemplates.css.
/// </summary>
public partial class EmailTemplateEditorControl : ComponentBase
{
    [Parameter, EditorRequired]
    public EmailTemplateFormModel Model { get; set; } = new();

    [Parameter]
    public string SubmitLabel { get; set; } = "Save Template";

    [Parameter]
    public EventCallback<EmailTemplateFormModel> OnSubmit { get; set; }

    [Parameter]
    public EventCallback<EmailTemplateFormModel> OnDelete { get; set; }

    private Task HandleValidSubmit() => OnSubmit.InvokeAsync(Model);

    private Task HandleDelete() => OnDelete.InvokeAsync(Model);
}
