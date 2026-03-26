using System.Threading.Channels;
using PersonFinder.Application.Abstractions;
using PersonFinder.Application.Common;

namespace PersonFinder.Infrastructure.BackgroundServices;

public sealed class BioQueue : IBioQueue
{
    private readonly Channel<BioGenerationItem> _channel =
        Channel.CreateUnbounded<BioGenerationItem>(new UnboundedChannelOptions
        {
            SingleReader = true
        });

    public void Enqueue(BioGenerationItem item) => _channel.Writer.TryWrite(item);

    public async Task<BioGenerationItem> DequeueAsync(CancellationToken cancellationToken) =>
        await _channel.Reader.ReadAsync(cancellationToken);
}
