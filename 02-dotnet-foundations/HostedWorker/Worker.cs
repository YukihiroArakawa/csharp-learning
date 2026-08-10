namespace HostedWorker;

public class Worker(
    ILogger<Worker> logger,
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var message = configuration["Worker:Message"]
            ?? throw new InvalidOperationException("Worker:Message is not configured.");

        logger.LogInformation("Configured message: {Message}", message);

        for (var scopeNumber = 1; scopeNumber <= 2; scopeNumber++)
        {
            using var scope = scopeFactory.CreateScope();
            LogLifetimeIds(scope.ServiceProvider, scopeNumber);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

    private void LogLifetimeIds(IServiceProvider services, int scopeNumber)
    {
        var singleton1 = services.GetRequiredService<SingletonId>();
        var singleton2 = services.GetRequiredService<SingletonId>();
        var scoped1 = services.GetRequiredService<ScopedId>();
        var scoped2 = services.GetRequiredService<ScopedId>();
        var transient1 = services.GetRequiredService<TransientId>();
        var transient2 = services.GetRequiredService<TransientId>();

        LogIds(scopeNumber, "singleton", singleton1.Id, singleton2.Id);
        LogIds(scopeNumber, "scoped", scoped1.Id, scoped2.Id);
        LogIds(scopeNumber, "transient", transient1.Id, transient2.Id);
    }

    private void LogIds(int scopeNumber, string lifetime, Guid firstId, Guid secondId)
    {
        logger.LogInformation(
            "Scope {ScopeNumber}, {Lifetime}: {FirstId} / {SecondId}",
            scopeNumber,
            lifetime,
            firstId,
            secondId);
    }
}
