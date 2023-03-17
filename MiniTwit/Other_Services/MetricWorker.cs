using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;
using Prometheus;

namespace MiniTwit.Other_Services;

public sealed class MetricWorker : BackgroundService
{
    private static readonly Gauge ActiveUsersGauge = Metrics
        .CreateGauge("minitwit_active_users", "Number users that have tweeted in the last 12 hours.");
    private static readonly Gauge MessagesCreated = Metrics
        .CreateGauge("minitwit_messages_created_time", "Summary of messages created over the last 30 minutes.");
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MetricWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
        
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var miniTwitContext = scope.ServiceProvider.GetService<MiniTwitContext>();
                var activeUsers = await miniTwitContext.Messages
                    .Where(m => m.PublishDate > DateTime.UtcNow.AddHours(-12))
                    .Select(m => m.AuthorId)
                    .Distinct()
                    .CountAsync(cancellationToken: stoppingToken);

                ActiveUsersGauge.Set(activeUsers);
                
                var messagesCreated = await miniTwitContext.Messages
                    .Where(m => m.PublishDate > DateTime.UtcNow.AddMinutes(-30))
                    .Select(m => m.Id)
                    .Distinct()
                    .CountAsync(cancellationToken: stoppingToken);

                MessagesCreated.Set(messagesCreated);

            }
            await Task.Delay(1_800_000, stoppingToken); //1,800,000 ms: 30 mins
        }
    }
}