using System.Collections.Concurrent;
using AsyncMessageSystem.Order.Dto;
using AsyncMessageSystem.Order.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AsyncMessageSystem.Order.Mappers;
using AsyncMessageSystem.ResultPattern;

namespace AsyncMessageSystem.Services;

public interface IOrderRepository
{
    Task<Result<OrderDto>> AddOrder(CreateOrderRequest order, CancellationToken cancellationToken);
    Task<Result<OrderDto>> GetOrderById(string id, CancellationToken cancellationToken);
    Task<Result<List<OrderDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<OrderDto>> UpdateStatus(string id, OrderStatus status, DateTime? processedAt = null);
}
public class OrderRepositoryPql(AppDbContext appDbContext) : IOrderRepository
{
    private readonly AppDbContext _DbCtx = appDbContext;
    public async Task<Result<OrderDto>> AddOrder(CreateOrderRequest orderRequest, CancellationToken cancellationToken)
    {

        var order = new OrderModel(
       Guid.NewGuid(),
       orderRequest.CustomerName,
       orderRequest.ProductName,
       orderRequest.Quantity);
        try
        {
            await this._DbCtx.AddAsync(order);
            await _DbCtx.SaveChangesAsync();
        }
        catch (Exception exc)
        {
            return Result<OrderDto>.Failed(exc.Message);
        }

        return Result<OrderDto>.Success(order.fromModelToDto());
    }

    public async Task<Result<List<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        List<OrderDto> listOrderDto = new();
        try
        {
            var allOrders = await this._DbCtx.Order
             .AsNoTracking()
             .ToListAsync();
            foreach (OrderModel orderModel in allOrders)
            {
                listOrderDto.Add(orderModel.fromModelToDto());
            }
        }
        catch (Exception exc)
        {
            return Result<List<OrderDto>>.Failed(exc.Message);
        }
        return Result<List<OrderDto>>.Success(listOrderDto);
    }

    public async Task<Result<OrderDto>> GetOrderById(string id, CancellationToken cancellationToken)
    {
        try
        {
            var orderByIdResult = await FindOrder(id);
            if (orderByIdResult.isAnyError)
            {
                return Result<OrderDto>.Failed("Order Not Found");
            }
            return Result<OrderDto>.Success(orderByIdResult.Value!.fromModelToDto());
        }
        catch (FormatException)
        {
            return Result<OrderDto>.Failed("Invalid GUID");
        }
        catch (Exception exc)
        {
            return Result<OrderDto>.Failed(exc.Message);
        }
    }

    public async Task<Result<OrderDto>> UpdateStatus(string id, OrderStatus status, DateTime? processedAt = null)
    {
        try
        {
            var orderById = await FindOrder(id);
            if (!orderById.isAnyError)
            {
                orderById.Value!.Status = status;
                orderById.Value.ProcessedAt = processedAt;
                await this._DbCtx.SaveChangesAsync();
                return Result<OrderDto>.Success(orderById.Value.fromModelToDto());
            }
            return Result<OrderDto>.Failed(orderById.Error!.Description);
        }
        catch (OperationCanceledException)
        {
            return Result<OrderDto>.Failed("Operation Cancelled");
        }
        catch (Exception)
        {
            return Result<OrderDto>.Failed("Error, Try Later.");
        }
    }
    private async Task<Result<OrderModel>> FindOrder(string id)
    {

        Guid idGuid = Guid.Parse(id);
        OrderModel? orderById = await this._DbCtx.Order
        .FirstOrDefaultAsync(x => x.Id == idGuid);
        if (orderById is null)
        {
            return Result<OrderModel>.Failed("Order not Found");
        }
        return Result<OrderModel>.Success(orderById);
    }
}
