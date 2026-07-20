using System.Security.Claims;
using BacklogTicketManager.Common;
using BacklogTicketManager.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BacklogTicketManager.Pages;

/// <summary>Sign-in page. Validates credentials via <see cref="IAuthService"/> and issues the
/// authentication cookie (which must happen in an HTTP request, hence a Razor Page).</summary>
[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public Common.LoginModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

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

        var user = await _authService.ValidateCredentialsAsync(Input.Username, Input.Password);
        if (user is null)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        await SignInAsync(HttpContext, user, Input.RememberMe);
        return LocalRedirect(string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);
    }

    /// <summary>Builds the claims principal and writes the auth cookie. Shared with registration.</summary>
    internal static Task SignInAsync(HttpContext httpContext, User user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new("DisplayName", user.DisplayName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        return httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = isPersistent });
    }
}
