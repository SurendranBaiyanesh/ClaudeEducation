using System.ComponentModel.DataAnnotations;

namespace BacklogTicketManager.Common;

/// <summary>
/// Editable, validated view of an <see cref="EmailTemplate"/> used by the template designer.
/// Kept separate from <see cref="EmailTemplate"/> so validation attributes (a UI concern) never
/// leak into the persistence model (SRP), mirroring <see cref="TicketFormModel"/>.
/// </summary>
public class EmailTemplateFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description must be 1000 characters or fewer.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Subject is required.")]
    [StringLength(400, ErrorMessage = "Subject must be 400 characters or fewer.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Body is required.")]
    public string Body { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public static EmailTemplateFormModel FromTemplate(EmailTemplate template) => new()
    {
        Id = template.Id,
        Name = template.Name,
        Description = template.Description,
        Subject = template.Subject,
        Body = template.Body,
        IsActive = template.IsActive
    };

    public EmailTemplate ToTemplate() => new()
    {
        Id = Id,
        Name = Name,
        Description = Description,
        Subject = Subject,
        Body = Body,
        IsActive = IsActive
    };
}
