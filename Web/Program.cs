using BacklogTicketManager.Data;
using BacklogTicketManager.Data.Mapping;
using BacklogTicketManager.Data.Options;
using BacklogTicketManager.Data.Repositories;
using BacklogTicketManager.Data.Schema;
using BacklogTicketManager.Logic;
using BacklogTicketManager.Logic.BackgroundServices;
using BacklogTicketManager.Logic.Options;
using BacklogTicketManager.Services;

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
// Data access layer - System.Data based, XML-defined DataTable schemas.
// Registered as scoped so each circuit/request gets its own SqlConnection.
// ---------------------------------------------------------------------
builder.Services.AddSingleton<IDataTableSchemaProvider, DataTableSchemaProvider>();
builder.Services.AddScoped<IDbConnectionFactory, SqlDbConnectionFactory>();

builder.Services.AddSingleton<ITicketMapper, TicketMapper>();
builder.Services.AddSingleton<ITicketUpdateMapper, TicketUpdateMapper>();
builder.Services.AddSingleton<IEmailNotificationMapper, EmailNotificationMapper>();
builder.Services.AddSingleton<IEmailTemplateMapper, EmailTemplateMapper>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketUpdateRepository, TicketUpdateRepository>();
builder.Services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();

// ---------------------------------------------------------------------
// Business / application services.
// ---------------------------------------------------------------------
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
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
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
