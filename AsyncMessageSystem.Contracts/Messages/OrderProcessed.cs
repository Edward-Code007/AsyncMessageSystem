namespace AsyncMessageSystem.Messages;

public record OrderProcessed
{
    public Guid OrderId { get; init; }
    public DateTime ProcessedAt { get; init; }
}
