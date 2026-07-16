using BacklogTicketManager.Logic.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BacklogTicketManager.Logic.BackgroundServices;

/// <summary>
/// Periodically scans for delayed / due-soon tickets and triggers automatic emails,
/// independent of any user browsing the app. Interval is configurable via
/// Notification:PollingIntervalMinutes.
/// </summary>
public sealed class DelayedTicketMonitorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly NotificationOptions _options;
    private readonly ILogger<DelayedTicketMonitorService> _logger;

    public DelayedTicketMonitorService(
        IServiceScopeFactory scopeFactory,
        IOptions<NotificationOptions> options,
        ILogger<DelayedTicketMonitorService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(Math.Max(1, _options.PollingIntervalMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // ITicketNotificationService and its dependencies (repositories) are scoped,
                // so a fresh scope is created for each polling cycle.
                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<ITicketNotificationService>();

                var sentCount = await notificationService.ProcessDueAndOverdueTicketsAsync(stoppingToken);
                if (sentCount > 0)
                {
                    _logger.LogInformation("Delayed ticket monitor sent {Count} notification email(s).", sentCount);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Delayed ticket monitor cycle failed.");
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown.
            }
        }
    }
}
