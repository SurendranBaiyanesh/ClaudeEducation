using BacklogTicketManager.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BacklogTicketManager.Pages;

/// <summary>Sign-up page. Creates the account via <see cref="IAuthService"/> (password hashed),
/// then signs the new user in and redirects to the app.</summary>
[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public Common.RegisterModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _authService.RegisterAsync(Input);
        if (!result.Succeeded || result.User is null)
        {
            ErrorMessage = result.Error ?? "Registration failed.";
            return Page();
        }

        await LoginModel.SignInAsync(HttpContext, result.User, isPersistent: false);
        return LocalRedirect("/");
    }
}
