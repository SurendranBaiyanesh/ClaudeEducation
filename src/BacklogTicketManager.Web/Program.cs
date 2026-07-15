using BacklogTicketManager.Data;
using BacklogTicketManager.Data.Mapping;
using BacklogTicketManager.Data.Options;
using BacklogTicketManager.Data.Repositories;
using BacklogTicketManager.Data.Schema;
using BacklogTicketManager.Logic;
using BacklogTicketManager.Logic.BackgroundServices;
using BacklogTicketManager.Logic.Options;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Options (bound from appsettings.json) - one class per concern (SRP).
// ---------------------------------------------------------------------
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<NotificationOptions>(builder.Configuration.GetSection("Notification"));

// ---------------------------------------------------------------------
// Blazor Server
// ---------------------------------------------------------------------
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// ---------------------------------------------------------------------
// Data access layer - System.Data based, XML-defined DataTable schemas.
// Registered as scoped so each circuit/request gets its own SqlConnection.
// ---------------------------------------------------------------------
builder.Services.AddSingleton<IDataTableSchemaProvider, DataTableSchemaProvider>();
builder.Services.AddScoped<IDbConnectionFactory, SqlDbConnectionFactory>();

builder.Services.AddSingleton<ITicketMapper, TicketMapper>();
builder.Services.AddSingleton<ITicketUpdateMapper, TicketUpdateMapper>();
builder.Services.AddSingleton<IEmailNotificationMapper, EmailNotificationMapper>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketUpdateRepository, TicketUpdateRepository>();
builder.Services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();

// ---------------------------------------------------------------------
// Business / application services.
// ---------------------------------------------------------------------
builder.Services.AddScoped<ITicketService, TicketService>();
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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
