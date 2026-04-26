using AsyncMessageSystem.Order.Dto;
using AsyncMessageSystem.Order.Model;
using AsyncMessageSystem.ResultPattern;
using AsyncMessageSystem.Services;
using MassTransit.Contracts.JobService;
using Microsoft.EntityFrameworkCore;
using Moq;
namespace IOrderRepo.Test;

public class OrderRepo_Test
{
    IOrderService _orderService;
    Mock<IOrderRepository> _orderRepository;
    CancellationTokenSource _cancellationTokenSource;

    public OrderRepo_Test()
    {


        this._orderRepository = new Mock<IOrderRepository>();
        this._orderService = new OrderSerPql(this._orderRepository.Object);
        this._cancellationTokenSource = new CancellationTokenSource();
    }

    [Fact]
    public async Task GetAllOrders_Should_ReturnResultAllOrders()
    {
        //Arrange
        List<OrderModel> orders = [
            new OrderModel(Guid.CreateVersion7(),"Felix","Pepino",10),
             new OrderModel(Guid.CreateVersion7(),"Ruso","Papa",10),
              new OrderModel(Guid.CreateVersion7(),"Moise","Guayaba",10),
        ];
        this._orderRepository.Setup( x=> x.GetAll(this._cancellationTokenSource.Token))
        .ReturnsAsync(orders);


        //Act
        var allOrders = await this._orderService.GetAllOrders(this._cancellationTokenSource.Token);

        //Assert
        Assert.IsType<Result<List<OrderDto>>>(allOrders);
        Assert.Equal(3,allOrders.Value!.Count);
    }
    [Fact]
    public async Task GetOrderByIdShouldReturnResultOrder()
    {
        //Arrange
        Guid id = Guid.CreateVersion7();
        OrderModel orderModel = new(id,"Felix","Pepino",10);
        this._orderRepository.Setup( x=> x.GetOrderById(id.ToString(),this._cancellationTokenSource.Token))
        .ReturnsAsync(orderModel);
        //Act
        var orderById = await this._orderService.GetOrderById(id.ToString(),this._cancellationTokenSource.Token);
        //Assert
        Assert.IsType<Result<OrderDto>>(orderById);
        Assert.Equal(orderById.Value!.id, id.ToString());

    }

    [Fact]
    public async Task AddOrder_Should_AddOrderandReturnResultOrderDTO()
    {
        //Arrange
        Guid id = Guid.CreateVersion7();
        OrderModel orderModel = new OrderModel(id,"Eduardo","Peras",10);
        CreateOrderRequest orderRequest = new CreateOrderRequest();
        this._orderRepository.Setup(x => x.AddOrder(orderRequest,this._cancellationTokenSource.Token))
        .ReturnsAsync(orderModel);

        //Act
        var addOrderResult = await this._orderService.InsertOrder(orderRequest,this._cancellationTokenSource.Token);

        //Assert
        Assert.IsType<Result<OrderDto>>(addOrderResult);
        Assert.Equal(10,addOrderResult.Value!.quantity);


    }
}