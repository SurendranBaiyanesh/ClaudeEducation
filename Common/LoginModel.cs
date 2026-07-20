using System.ComponentModel.DataAnnotations;

namespace BacklogTicketManager.Common;

/// <summary>Validated credentials submitted from the sign-in page.</summary>
public class LoginModel
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
