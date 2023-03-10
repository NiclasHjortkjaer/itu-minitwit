using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;
using Prometheus;

namespace MiniTwit.Hubs;

public sealed class MetricWorker : BackgroundService
{
    private static readonly Gauge ActiveUsersGauge = Metrics
        .CreateGauge("minitwit_active_users", "Number users that have tweeted in the last 12 hours.");
    private readonly MiniTwitContext _miniTwitContext;

    public MetricWorker(MiniTwitContext miniTwitContext)
    {
        _miniTwitContext = miniTwitContext;
    }
        
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var activeUsers = await _miniTwitContext.Messages
                .Where(m => m.PublishDate > DateTime.UtcNow.AddHours(-12))
                .Select(m => m.AuthorId)
                .Distinct()
                .CountAsync(cancellationToken: stoppingToken);

            ActiveUsersGauge.Set(activeUsers);

            await Task.Delay(1_800_000, stoppingToken); //1,800,000 ms: 30 mins
        }
    }
}