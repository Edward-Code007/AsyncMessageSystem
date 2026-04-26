using System.Collections.Concurrent;
using AsyncMessageSystem.Order.Dto;
using AsyncMessageSystem.Order.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AsyncMessageSystem.Order.Mappers;
using AsyncMessageSystem.ResultPattern;
using System.Reflection.Metadata.Ecma335;
using System.Globalization;

namespace AsyncMessageSystem.Services;


public class OrderSerPql(IOrderRepository _orderRepo): IOrderService
{

    public async Task<Result<OrderDto>> InsertOrder(CreateOrderRequest orderRequest, CancellationToken cancellationToken)
    {


        try
        {
          var result = await _orderRepo.AddOrder(orderRequest,cancellationToken);
            if (result.isAnyError)
            {
                return result.Error!.Description;
            }
            return result.Value!.fromModelToDto();
        }
        catch (Exception exc)
        {
            return exc.Message;
        }

    }

    public async Task<Result<List<OrderDto>>> GetAllOrders(CancellationToken cancellationToken)
    {
        List<OrderDto> listOrderDto = new();
        try
        {
            var allOrders = await _orderRepo.GetAll(cancellationToken);
            foreach (OrderModel orderModel in allOrders.Value!)
            {
                listOrderDto.Add(orderModel.fromModelToDto());
            }
        }
        catch (Exception exc)
        {
            return exc.Message;
        }
        return listOrderDto;
    }

    public async Task<Result<OrderDto>> GetOrderById(string id, CancellationToken cancellationToken)
    {
        try
        {
            var orderByIdResult = await _orderRepo.GetOrderById(id,cancellationToken);
            if (orderByIdResult.isAnyError)
            {
                return orderByIdResult.Error!.Description;
            }
            return orderByIdResult.Value!.fromModelToDto();
        }
        catch (FormatException)
        {
            return "Invalid GUID";
        }
        catch (Exception exc)
        {
            return exc.Message;
        }
    }

    public async Task<Result<OrderDto>> UpdateStatus(string id, OrderStatus status, DateTime? processedAt = null)
    {
        try
        {
            var updateResult = await _orderRepo.UpdateStatus(id,status,processedAt);
            if (updateResult.isAnyError)
            {
            return Result<OrderDto>.Failed(updateResult.Error!.Description);
            }
                else return updateResult.Value!.fromModelToDto();
        }
        catch (OperationCanceledException)
        {
            return "Operation Cancelled";
        }
        catch (Exception)
        {
           return "Error Try Later";
        }
    }
}

public interface IOrderService
{
    public Task<Result<OrderDto>> InsertOrder(CreateOrderRequest orderRequest, CancellationToken cancellationToken);
    public  Task<Result<List<OrderDto>>> GetAllOrders(CancellationToken cancellationToken);
     public Task<Result<OrderDto>> GetOrderById(string id, CancellationToken cancellationToken);
     public Task<Result<OrderDto>> UpdateStatus(string id, OrderStatus status, DateTime? processedAt = null);
}
