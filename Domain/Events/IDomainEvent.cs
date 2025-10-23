namespace Domain.Events
{
    /// <summary>
    /// Basic interface for a domain event.
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
        Guid Id { get; }
    }
}