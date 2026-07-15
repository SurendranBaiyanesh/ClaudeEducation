using BacklogTicketManager.Common;
using Microsoft.AspNetCore.Components;

namespace BacklogTicketManager.Shared.Controls;

/// <summary>
/// Create/edit form for a ticket. Validation is declarative (DataAnnotations on
/// <see cref="TicketFormModel"/>); this control only raises <see cref="OnSubmit"/> with the
/// validated model - it never talks to services or repositories directly.
/// </summary>
public partial class TicketFormControl : ComponentBase
{
    [Parameter, EditorRequired]
    public TicketFormModel Model { get; set; } = new();

    [Parameter]
    public string SubmitLabel { get; set; } = "Save";

    [Parameter]
    public EventCallback<TicketFormModel> OnSubmit { get; set; }

    private Task HandleValidSubmit() => OnSubmit.InvokeAsync(Model);
}
