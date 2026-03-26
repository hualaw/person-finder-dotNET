using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersonFinder.Application.Abstractions;
using PersonFinder.Domain.Repositories;

namespace PersonFinder.Infrastructure.BackgroundServices;

public sealed class BioGenerationBackgroundService : BackgroundService
{
    private readonly IBioQueue _bioQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BioGenerationBackgroundService> _logger;

    public BioGenerationBackgroundService(
        IBioQueue bioQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<BioGenerationBackgroundService> logger)
    {
        _bioQueue = bioQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var item = await _bioQueue.DequeueAsync(stoppingToken);

                // Each bio generation runs in its own DI scope so it gets a fresh DbContext
                await using var scope = _scopeFactory.CreateAsyncScope();
                var geminiService = scope.ServiceProvider.GetRequiredService<IGeminiService>();
                var personRepository = scope.ServiceProvider.GetRequiredService<IPersonRepository>();

                var bio = await geminiService.GenerateBioAsync(item.JobTitle, item.Hobbies, stoppingToken);
                await personRepository.UpdateBioAsync(item.PersonId, bio, stoppingToken);

                _logger.LogInformation(
                    "event=BioGenerated personId={PersonId}",
                    item.PersonId);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "event=BioGenerationFailed error={Error}",
                    ex.Message);
            }
        }
    }
}
