using AsyncMessageSystem.Order.Dto;
using AsyncMessageSystem.Order.Mappers;
using AsyncMessageSystem.Order.Model;
using AsyncMessageSystem.ResultPattern;
using AsyncMessageSystem.Services;
using Microsoft.EntityFrameworkCore;

public class OrderRep(AppDbContext dbCtx) : IOrderRepository
{
    public async Task<Result<OrderModel>> AddOrder(CreateOrderRequest orderRequest, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            var newOrder = new OrderModel(
                Guid.CreateVersion7(),
                orderRequest.CustomerName,
                orderRequest.ProductName,
                orderRequest.Quantity);

            dbCtx.Add(newOrder);
            await dbCtx.SaveChangesAsync();

            return newOrder;
        }
        else
        {
            return "Orden Cancelada";
        }
    }

    public async Task<Result<List<OrderModel>>> GetAll(CancellationToken cancellationToken)
    {

        var users = await dbCtx.Order
        .AsNoTracking()
        .ToListAsync();

        return users;
    }

    public async Task<Result<OrderModel>> GetOrderById(string id, CancellationToken cancellationToken)
    {
        var order = await FindOrder(id);
        if (order.isAnyError)
        {
            return order.Error!.Description;
        }
        else return order.Value!;
    }

    public async Task<Result<OrderModel>> UpdateStatus(string id, OrderStatus status, DateTime? processedAt = null)
    {
        var order = await FindOrder(id);
        if (order.isAnyError)
        {
            return order.Error!.Description;
        }
        order.Value!.ProcessedAt = processedAt;
        order.Value!.Status = status;
        await dbCtx.SaveChangesAsync();
        return order.Value;

    }

    private async Task<Result<OrderModel>> FindOrder(string id)
    {

        Guid idGuid = Guid.Parse(id);
        OrderModel? orderById = await dbCtx.Order
        .FirstOrDefaultAsync(x => x.Id == idGuid);
        if (orderById is null)
        {
            return "Order Not Found";
        }
        return orderById;
    }
}
public interface IOrderRepository
{
    Task<Result<OrderModel>> AddOrder(CreateOrderRequest order, CancellationToken cancellationToken);
    Task<Result<OrderModel>> GetOrderById(string id, CancellationToken cancellationToken);
    Task<Result<List<OrderModel>>> GetAll(CancellationToken cancellationToken);
    Task<Result<OrderModel>> UpdateStatus(string id, OrderStatus status, DateTime? processedAt = null);
}