namespace ServiceA.Controllers;

using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Services.TransactionStorage;
using Shared.Messages;
using Shared.Protos;

[ApiController]
public class TransactionController(
    ILogger<TransactionController> logger,
    TransactionStorage storage,
    ServiceC.ServiceCClient serviceCClient,
    IPublishEndpoint publishEndpoint
    ) : ControllerBase
{

    [HttpPost("api/start")]
    public async Task<IActionResult> Start(CancellationToken cancellationToken)
    {
        var transactionId = Guid.NewGuid();

        try
        {
            storage.Create(transactionId);
            
            await Task.WhenAll(
                publishEndpoint.Publish(new ProcessInServiceA(transactionId), cancellationToken),
                publishEndpoint.Publish(new ProcessInServiceB(transactionId), cancellationToken)
            );

            await storage.WaitAsync(transactionId, cancellationToken);

            var state = storage.Get(transactionId)
                        ?? throw new InvalidOperationException("Transaction state missing after completion");

            var grpcRequest = new FinalizeRequest
            {
                TransactionId = state.Id.ToString(),
                ServiceA = state.FromA,
                ServiceB = state.FromB,
                Timestamp = state.CreatedAt.ToString("O")
            };

            var grpcResponse = await serviceCClient.FinalizeTransactionAsync(grpcRequest);

            return Ok(grpcResponse);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error processing transaction {TransactionId}", transactionId);

            return StatusCode(500, new
            {
                Success = false,
                Message = $"Error processing transaction: {e.Message}"
            });
        }
        finally
        {
            storage.Remove(transactionId);
        }
    }
}