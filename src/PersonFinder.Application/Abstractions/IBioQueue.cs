using PersonFinder.Application.Common;

namespace PersonFinder.Application.Abstractions;

public interface IBioQueue
{
    void Enqueue(BioGenerationItem item);
    Task<BioGenerationItem> DequeueAsync(CancellationToken cancellationToken);
}
