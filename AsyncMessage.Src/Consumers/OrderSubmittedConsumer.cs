using MassTransit;
using AsyncMessageSystem.Messages;
using AsyncMessageSystem.Order.Model;
using AsyncMessageSystem.Services;

namespace AsyncMessageSystem.Consumers;

public class OrderSubmittedConsumer(
    IOrderService orderService,
    IPublishEndpoint publishEndpoint,
    ILogger<OrderSubmittedConsumer> logger) : IConsumer<OrderSubmitted>
{
    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        var message = context.Message;
        logger.LogInformation(
            "Processing order {OrderId} for {Customer} - {Product} x{Quantity}",
            message.OrderId, message.CustomerName, message.ProductName, message.Quantity);

        string orderId = message.OrderId.ToString();
        await orderService.UpdateStatus(orderId, OrderStatus.Processing);

        // Simulate heavy processing (3-5 seconds)
        var processingTime = Random.Shared.Next(3000, 5001);
        await Task.Delay(processingTime, context.CancellationToken);

        var processedAt = DateTime.UtcNow;
        await orderService.UpdateStatus(orderId, OrderStatus.Processed, processedAt);

        await publishEndpoint.Publish(new OrderProcessed
        {
            OrderId = message.OrderId,
            ProcessedAt = processedAt
        }, context.CancellationToken);

        logger.LogInformation(
            "Order {OrderId} processed successfully in {ElapsedMs}ms",
            orderId, processingTime);
    }
}
