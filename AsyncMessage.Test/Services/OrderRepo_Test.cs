using AsyncMessageSystem.Order.Dto;
using AsyncMessageSystem.Order.Model;
using AsyncMessageSystem.ResultPattern;
using AsyncMessageSystem.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
namespace IOrderRepo.Test;

public class OrderRepo_Test
{
    AppDbContext _dbMock;
    IOrderRepository _orderRepo;
    CancellationTokenSource _cancellationTokenSource;
    public OrderRepo_Test()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.CreateVersion7().ToString()).Options;

        this._dbMock = new AppDbContext(options);
        this._orderRepo = new OrderRepositoryPql(_dbMock);
        this._cancellationTokenSource = new CancellationTokenSource();
    }

    [Fact]
    public async Task GetAllOrders_Should_ReturnResultAllOrders()
    {
        //Arrange
        _dbMock.AddRange([
            new OrderModel(Guid.CreateVersion7(),"Eduardo","FrutaBomba",10),
            new OrderModel(Guid.CreateVersion7(),"Felix","Pepino",10),
            new OrderModel(Guid.CreateVersion7(),"Moise","Papaya",10),
        ]);
        _dbMock.SaveChanges();
        CancellationToken cancellationToken = this._cancellationTokenSource.Token;

        //Act
        var result = await _orderRepo.GetAll(cancellationToken);

        //Assert
        Assert.IsType<Result<List<OrderDto>>>(result);
        Assert.NotEmpty(result.Value!);
        Assert.Equal(3, result.Value!.Count);
    }
    [Fact]
    public async Task GetOrderByIdShouldReturnResultOrder()
    {
        //Arrange
        Guid id = Guid.CreateVersion7();
        _dbMock.Add(new OrderModel(id, "Eduardo", "FrutaBomba", 10));
        _dbMock.SaveChanges();
        //Act
        CancellationToken cancellationToken = this._cancellationTokenSource.Token;
        var result = await _orderRepo.GetOrderById(id.ToString(), cancellationToken);
        //Assert
        Assert.IsType<Result<OrderDto>>(result);
        Assert.False(result.isAnyError);
        Assert.Equal(id.ToString(), result.Value!.id);
    }

    [Fact]
    public async Task AddOrder_Should_AddOrderandReturnResultOrderDTO()
    {
        //Arrange
        CreateOrderRequest newOrder = new CreateOrderRequest()
        {
            CustomerName = "Eduardo",
            ProductName = "Zapato",
            Quantity = 1
        };
        CancellationToken cancellationToken = this._cancellationTokenSource.Token;
        //Act
        var result = await this._orderRepo.AddOrder(newOrder, cancellationToken);
        //Assert
        Assert.IsType<Result<OrderDto>>(result);
        Assert.False(result.isAnyError);
        Assert.Equal("Eduardo", result.Value!.customerName);


    }
}