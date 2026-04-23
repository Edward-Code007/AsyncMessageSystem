namespace AsyncMessageSystem.Order.Model;

public enum OrderStatus
{
    Submitted,
    Processing,
    Processed,
    Failed
}

public class OrderModel(Guid id, string customerName, string productName, int quantity)
{
    public Guid Id { get; set; } = id;
    public string CustomerName { get; set; } = customerName;
    public string ProductName { get; set; } = productName;
    public int Quantity { get; set; } = quantity;
    public OrderStatus Status { get; set; } = OrderStatus.Submitted;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
