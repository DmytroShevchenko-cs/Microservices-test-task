namespace ServiceA.Consumers;

using MassTransit;
using Services.TransactionStorage;
using Shared.Messages;

public class ServiceBProcessConsumer(
    ILogger<ServiceBProcessConsumer> logger,
    TransactionStorage storage
    ) : IConsumer<ProcessInServiceB>
{

    public Task Consume(ConsumeContext<ProcessInServiceB> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Received response from ServiceB for transaction {TransactionId}",
            message.TransactionId);

        var state = storage.Get(message.TransactionId);

        if (state == null)
        {
            logger.LogWarning(
                "Received response for unknown transaction {TransactionId}",
                message.TransactionId);
            
            return Task.CompletedTask;
        }

        state.FromB = true;

        storage.TryComplete(message.TransactionId);
        
        return Task.CompletedTask;
    }
}
