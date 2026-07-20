using BacklogTicketManager.Data;
using BacklogTicketManager.Data.Mapping;
using BacklogTicketManager.Data.Options;
using BacklogTicketManager.Data.Repositories;
using BacklogTicketManager.Data.Schema;
using BacklogTicketManager.Logic;
using BacklogTicketManager.Logic.BackgroundServices;
using BacklogTicketManager.Logic.Options;
using BacklogTicketManager.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Options (bound from appsettings.json) - one class per concern (SRP).
// ---------------------------------------------------------------------
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<NotificationOptions>(builder.Configuration.GetSection("Notification"));
builder.Services.Configure<CurrentUserOptions>(builder.Configuration.GetSection("CurrentUser"));

// ---------------------------------------------------------------------
// Blazor Server
// ---------------------------------------------------------------------
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Bundles every wwwroot/css/*.css file into a single stylesheet served at /css/bundle.css.
builder.Services.AddSingleton<IStyleBuilder, StyleBuilder>();

// ---------------------------------------------------------------------
// Authentication - custom cookie auth backed by the Users table.
// ---------------------------------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
// Secure by default: every endpoint requires an authenticated user unless it is
// explicitly marked [AllowAnonymous] (Login/Register pages, logout, CSS bundle).
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddHttpContextAccessor();

// ---------------------------------------------------------------------
// Data access layer - System.Data based, XML-defined DataTable schemas.
// Registered as scoped so each circuit/request gets its own SqlConnection.
// ---------------------------------------------------------------------
builder.Services.AddSingleton<IDataTableSchemaProvider, DataTableSchemaProvider>();
builder.Services.AddScoped<IDbConnectionFactory, SqlDbConnectionFactory>();

builder.Services.AddSingleton<ITicketMapper, TicketMapper>();
builder.Services.AddSingleton<ITicketUpdateMapper, TicketUpdateMapper>();
builder.Services.AddSingleton<IEmailNotificationMapper, EmailNotificationMapper>();
builder.Services.AddSingleton<IEmailTemplateMapper, EmailTemplateMapper>();
builder.Services.AddSingleton<IUserMapper, UserMapper>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketUpdateRepository, TicketUpdateRepository>();
builder.Services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ---------------------------------------------------------------------
// Business / application services.
// ---------------------------------------------------------------------
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailTemplateBuilder, EmailTemplateBuilder>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<ITicketNotificationService, TicketNotificationService>();

// Background monitor that raises delay / due-soon emails automatically.
builder.Services.AddHostedService<DelayedTicketMonitorService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Signs the current user out and returns them to the sign-in page.
app.MapPost("/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/login");
}).AllowAnonymous();

// Serves the concatenated CSS bundle produced by StyleBuilder.
app.MapGet("/css/bundle.css", (IStyleBuilder styles, HttpContext http) =>
{
    var etag = $"\"{styles.ContentHash}\"";
    http.Response.Headers.CacheControl = "no-cache";

    if (http.Request.Headers.IfNoneMatch == etag)
    {
        return Results.StatusCode(StatusCodes.Status304NotModified);
    }

    http.Response.Headers.ETag = etag;
    return Results.Text(styles.BuildBundle(), "text/css");
}).AllowAnonymous();

app.MapBlazorHub();

// The _Host page (and every Blazor route it serves) is protected by the fallback
// authorization policy above; anonymous requests are redirected to /login by the cookie
// middleware. The Login/Register Razor Pages are [AllowAnonymous].
app.MapFallbackToPage("/_Host");

// Seed a default admin account on first run so the app is usable immediately.
// Credentials are for local/dev use only - change or remove for production.
using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    try
    {
        await authService.EnsureDefaultAdminAsync(
            username: "admin",
            email: "admin@backlog.local",
            displayName: "Administrator",
            password: "Admin@123");
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Could not seed the default admin user (is the database reachable?).");
    }
}

app.Run();
