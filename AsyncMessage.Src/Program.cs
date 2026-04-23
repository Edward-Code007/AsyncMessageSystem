using MassTransit;
using AsyncMessageSystem.Consumers;
using AsyncMessageSystem.Order.Dto;
using AsyncMessageSystem.Messages;
using AsyncMessageSystem.Services;
using AsyncMessageSystem.Order.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderRepository, OrderRepositoryPql>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderSubmittedConsumer>();
    x.AddConsumer<OrderProcessedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");

        cfg.Host(rabbitConfig["Host"] ?? "localhost", "/", h =>
        {
            h.Username(rabbitConfig["Username"] ?? "guest");
            h.Password(rabbitConfig["Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var orders = app.MapGroup("api/orders");

orders.MapPost("/", async (
    CreateOrderRequest request,
    IOrderRepository orderRepository,
    IPublishEndpoint publishEndpoint,
    ILogger<Program> logger,
    CancellationToken cancellationToken
    ) =>
{

    var result = await orderRepository.AddOrder(request,cancellationToken);
    
    if (!result.isAnyError)
    {
    await publishEndpoint.Publish(new OrderSubmitted
    {
        OrderId = Guid.Parse(result.Value!.id),
        CustomerName = result.Value!.customerName,
        ProductName = result.Value.productName,
        Quantity = result.Value.quantity,
        SubmittedAt = DateTime.UtcNow
    });

    logger.LogInformation("Order {OrderId} submitted by {Customer}", result.Value.id, result.Value.customerName);

    return Results.Accepted($"/api/orders/{result.Value.id}", new { result.Value.id, result.Value.status });
        
    }
    return Results.Problem(result!.Error!.Description);
});

orders.MapGet("/{id:guid}", async (string id, IOrderRepository orderRepository,CancellationToken cancellationToken) =>
{
    var orderResult = await orderRepository.GetOrderById(id,cancellationToken);
    return orderResult;
});

orders.MapGet("/", async (IOrderRepository orderRepository,CancellationToken cancellationToken) =>
{
    var result = await orderRepository.GetAll(cancellationToken);
    return result;
});

app.Run();
