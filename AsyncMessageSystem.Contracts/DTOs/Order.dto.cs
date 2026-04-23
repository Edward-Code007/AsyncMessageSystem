using System.ComponentModel.DataAnnotations;

namespace AsyncMessageSystem.Order.Dto;

public record CreateOrderRequest
{
    [Required]
    public string CustomerName { get; init; } = string.Empty;
    [Required]
    public string ProductName { get; init; } = string.Empty;
    [Range(1, 20)]
    public int Quantity { get; init; }
}
public record OrderDto(string id,string customerName,string productName,int quantity,string status){}