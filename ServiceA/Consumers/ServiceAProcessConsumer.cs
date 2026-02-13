namespace ServiceA.Consumers;

using MassTransit;
using Services.TransactionStorage;
using Shared.Messages;

public class ServiceAProcessConsumer(
    ILogger<ServiceAProcessConsumer> logger,
    TransactionStorage storage
) : IConsumer<ProcessInServiceA>
{
    public Task Consume(ConsumeContext<ProcessInServiceA> context)
    {
        var message = context.Message;
        logger.LogInformation("ServiceA processing transaction {TransactionId}",
            message.TransactionId);

        var state = storage.Get(message.TransactionId);

        if (state == null)
        {
            logger.LogWarning(
                "Received response for unknown transaction {TransactionId}",
                message.TransactionId);

            return Task.CompletedTask;
        }

        state.FromA = true;
        
        storage.TryComplete(message.TransactionId);
        
        return Task.CompletedTask;
    }
}