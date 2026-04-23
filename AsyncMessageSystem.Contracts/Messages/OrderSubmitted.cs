namespace AsyncMessageSystem.Messages;

public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public DateTime SubmittedAt { get; init; }
}
