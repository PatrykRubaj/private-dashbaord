using Core.DataAccess;
using Core.Model.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Services;

public class AirGradientPollingService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly AirGradientOptions _options;
    private readonly ILogger<AirGradientPollingService> _logger;

    public AirGradientPollingService(
        IServiceProvider services,
        IOptions<AirGradientOptions> options,
        ILogger<AirGradientPollingService> logger)
    {
        _services = services;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_options.PollingIntervalSeconds <= 0)
        {
            _logger.LogInformation("AirGradient polling disabled (PollingIntervalSeconds <= 0).");
            return;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.PollingIntervalSeconds));

        await PollAllAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await PollAllAsync(stoppingToken);
        }
    }

    private async Task PollAllAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var client = scope.ServiceProvider.GetRequiredService<IAirGradientClient>();

        var activeSensors = await dataContext.Sensors
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);

        var loggedCount = 0;
        foreach (var sensor in activeSensors)
        {
            var log = await client.FetchCurrentAsync(sensor, cancellationToken);
            if (log is null)
            {
                continue;
            }

            await dataContext.AirQualityLogs.AddAsync(log, cancellationToken);
            loggedCount++;
        }

        if (loggedCount > 0)
        {
            await dataContext.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Logged {Count} AirGradient readings.", loggedCount);
        }
    }
}
