using MassTransit;
using AsyncMessageSystem.Messages;

namespace AsyncMessageSystem.Consumers;

public class OrderProcessedConsumer(
    ILogger<OrderProcessedConsumer> logger) : IConsumer<OrderProcessed>
{
    public Task Consume(ConsumeContext<OrderProcessed> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Order {OrderId} completed at {ProcessedAt:O}. Ready for dispatch.",
            message.OrderId, message.ProcessedAt);

        return Task.CompletedTask;
    }
}
