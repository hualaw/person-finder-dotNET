namespace PersonFinder.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
