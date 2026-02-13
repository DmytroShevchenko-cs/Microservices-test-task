namespace ServiceB.Consumers;

using MassTransit;
using Shared.Messages;

public class ProcessInServiceBConsumer(
    ILogger<ProcessInServiceBConsumer> logger, 
    IPublishEndpoint publishEndpoint
    ) : IConsumer<ProcessInServiceB>
{
    public async Task Consume(ConsumeContext<ProcessInServiceB> context)
    {
        var message = context.Message;
        
        logger.LogInformation("ServiceB processing transaction {MessageTransactionId}", 
            message.TransactionId);
        
        var response = new ProcessInServiceB(message.TransactionId);
        
        await publishEndpoint.Publish(response);
    }
}
