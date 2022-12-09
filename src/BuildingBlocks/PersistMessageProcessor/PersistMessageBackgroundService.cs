using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.PersistMessageProcessor;

public class PersistMessageBackgroundService : BackgroundService
{
    private readonly ILogger<PersistMessageBackgroundService> _logger;
    private readonly IPersistMessageProcessor _persistMessageProcessor;
    private PersistMessageOptions _options;

    private Task? _executingTask;

    public PersistMessageBackgroundService(
        ILogger<PersistMessageBackgroundService> logger,
        IPersistMessageProcessor persistMessageProcessor,
        IOptions<PersistMessageOptions> options)
    {
        _logger = logger;
        _persistMessageProcessor = persistMessageProcessor;
        _options = options.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PersistMessage Background Service Start");

        _executingTask = ProcessAsync(stoppingToken);

        return _executingTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("PersistMessage Background Service Stop");

        return base.StopAsync(cancellationToken);
    }

    private async Task ProcessAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _persistMessageProcessor.ProcessAllAsync(stoppingToken);

                var delay = _options.Interval is { }
                    ? TimeSpan.FromSeconds((int)_options.Interval)
                    : TimeSpan.FromSeconds(30);

                await Task.Delay(delay, stoppingToken);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
