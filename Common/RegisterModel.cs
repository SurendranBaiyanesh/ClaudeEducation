using System.ComponentModel.DataAnnotations;

namespace BacklogTicketManager.Common;

/// <summary>Validated details submitted from the sign-up page to create a new account.</summary>
public class RegisterModel
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be 3-100 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required.")]
    [StringLength(200, ErrorMessage = "Display name must be 200 characters or fewer.")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(200, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
