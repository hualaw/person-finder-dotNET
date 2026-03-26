namespace PersonFinder.Domain.Events;

public sealed record PersonCreatedDomainEvent(long PersonId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
