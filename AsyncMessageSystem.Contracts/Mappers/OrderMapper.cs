using AsyncMessageSystem.Order.Dto;
using AsyncMessageSystem.Order.Model;

namespace AsyncMessageSystem.Order.Mappers;

public static class OrderMappers
{
    extension(OrderModel orderModel)
    {
    public OrderDto fromModelToDto()
    {
        string idOrder = orderModel.Id.ToString("");
        OrderDto orderDto = new OrderDto(
            idOrder,
            orderModel.CustomerName,
            orderModel.ProductName,
            orderModel.Quantity,
            orderModel.Status.ToString()
            );
        return orderDto;
    }
    }
    extension(OrderDto)
    {
        
    }

    
}